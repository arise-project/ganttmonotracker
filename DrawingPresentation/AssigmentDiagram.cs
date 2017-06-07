//----------------------------------------------------------------------------------------------
// <copyright file="AssigmentDiagram.cs" company="Artificial Renassance Inner Selft">
// Copyright (c) Artificial Renassance Inner Selft.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 04.02.2006 at 18:33

namespace GanttMonoTracker.DrawingPresentation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Cairo;

    using GanttTracker;
    using GanttTracker.TaskManager;
    using Gdk;

    using Pango;

    using TaskManagerInterface;

    public class AssigmentDiagramm : Gtk.DrawingArea, IGuiSource
    {
        private const int FBorderMarginH = 2;
        private const int FBorderMarginV = 2;
        private const int FTaskHeight = 14;

        public DataSet Source
        {
            get;
            set;
        }

        protected override bool OnExposeEvent(EventExpose args)
        {
            var baseResult = base.OnExposeEvent (args);
            if(TrackerCore.Instance.TaskManager is EmptyTaskManager)
            {
                return baseResult;
            }

            Source = TrackerCore.Instance.TaskManager.AssigmentSource;

            var v1 = Source.Tables["DataRange"].Rows[0]["MinDate"];
            var firstDate = v1 as DateTime? ?? DateTime.Now;
            var v2 = Source.Tables["DataRange"].Rows[0]["MaxDate"];
            var lastDate = v2 as DateTime? ?? DateTime.Now;
            var deltaSpan = lastDate.Subtract(firstDate);

            // Insert drawing code here.
            var grw = Gdk.CairoHelper.Create (GdkWindow);

            int fX, fY, fWidth, fHeight, fDepth;

            GdkWindow.GetGeometry(out fX,out fY,out fWidth,out fHeight,out fDepth);

            fWidth -= 3;
            fHeight -= 3;

            var delta = (deltaSpan.Days > 0) ?
                        (fWidth - 2 * FBorderMarginH) / deltaSpan.Days :
                        (fWidth - 2 * FBorderMarginH);

            GdkWindow.ClearArea(fX,fY,fWidth,fHeight);
            Show();

            //DrawBorder
            grw.SetSourceRGB(0xff, 0, 0);

            grw.MoveTo(FBorderMarginH, FBorderMarginV);
            grw.LineTo(fWidth - FBorderMarginH, FBorderMarginV);
            grw.LineTo(fWidth - FBorderMarginH, fHeight - FBorderMarginV);
            grw.LineTo(FBorderMarginH, fHeight - FBorderMarginV);
            grw.LineTo(FBorderMarginH, FBorderMarginV);
            grw.Stroke();

            //DrawTasks
            var deltaActor = (Source.Tables["Actor"].Rows.Count > 0) ?
                             (fHeight - 2 * FBorderMarginV) / Source.Tables["Actor"].Rows.Count :
                             fHeight - 2 * FBorderMarginV;
            deltaActor -= FTaskHeight;

            var maxTaskCout = (from DataRow row in Source.Tables["AssigmentSource"].Rows select (int) row["TaskCount"]).Concat(new[] {0}).Max();

            var actorIndex = 0;
            var foregroundColor3 = new Gdk.Color(0xff, 0xff, 0xff);
            var taskLabelGc = new Gdk.GC (GdkWindow);
            var colormap = Colormap.System;
            colormap.AllocColor(ref foregroundColor3,true,true);
            taskLabelGc.Foreground = foregroundColor3;

            //draw proportion line.
            var states1 = new Dictionary<int, int>();
            foreach (DataRow row in Source.Tables ["StateRange"].Rows)
            {
                v1 = row ["StateID"];
                var id = v1 as int? ?? -1;
                if (states1.ContainsKey (id))
                {
                    states1 [id] += 1;
                }
                else
                {
                    states1.Add (id, 0);
                }
            }

            var sum = states1.Keys.Sum(id => states1[id]);

            var states2 = states1.Keys.ToList();

            foreach (var id in states2.OrderBy(s => s))
            {
                states1 [id] = (int)(((double)fHeight / sum) * states1 [id]);
            }

            var proportionOffset = 0;
            foreach(var id in states1.Keys)
            {
                var state = Source.Tables ["StateRange"].Select ("StateID = " + id) [0];
                var taskGc = new Gdk.GC(GdkWindow);
                try
                {
                    var colorRed = Convert.ToByte(state["ColorRed"]);
                    var colorGreen = Convert.ToByte(state["ColorGreen"]);
                    var colorBlue = Convert.ToByte(state["ColorBlue"]);
                
                    var foregroundColor2 = new Gdk.Color(colorRed, colorGreen, colorBlue);
                    colormap.AllocColor(ref foregroundColor2,true,true);
                    taskGc.Foreground = foregroundColor2;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                GdkWindow.DrawRectangle (taskGc, true, fWidth - 10, proportionOffset, 10, states1 [id]);
                proportionOffset += states1 [id];
            }

            foreach(DataRow row in Source.Tables["Actor"].Rows)
            {
                var labelDate = firstDate;
                var offset = FBorderMarginH;
                for (var i = 0; i < deltaSpan.Days; i++)
                {
                    var taskCount = 0;
                    var assignmentId = -1;
                    var assignment = Source.Tables["AssigmentSource"]
                                     .Select("ActorID = " + row["ID"] + "and Date = '" +labelDate.ToShortDateString() + "'");
                    if (assignment.Length > 0)
                    {
                        v1 = assignment[0]["TaskCount"];
                        taskCount = v1 as int? ?? -1;
                        v2 = assignment[0]["ID"];
                        assignmentId = v2 as int? ?? -1;
                    }

                    if (taskCount > 0)
                    {
                        var states = Source.Tables ["StateRange"].Select ("AssigmentID = " + assignmentId, "StateID"); //order by stateid
                        var taskSum = 0;
                        foreach (var state in states)
                        {
                            var taskGc = new Gdk.GC(GdkWindow);
                            try
                            {
                                var colorRed = Convert.ToByte(state["ColorRed"]);
                                var colorGreen = Convert.ToByte(state["ColorGreen"]);
                                var colorBlue = Convert.ToByte(state["ColorBlue"]);
                                var foregroundColor2 = new Gdk.Color(colorRed, colorGreen, colorBlue);
                                colormap.AllocColor(ref foregroundColor2,true,true);
                                taskGc.Foreground = foregroundColor2;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                            
                            var currentCount = (int)state["TaskCount"];

                            GdkWindow.DrawRectangle(
                                taskGc,
                                true,
                                offset,
                                FBorderMarginV + (int)(deltaActor*(1 - (double)(taskSum + currentCount) / maxTaskCout))
                                + deltaActor * actorIndex
                                ,delta
                                ,(int)(deltaActor*((double)currentCount / maxTaskCout)));

                            taskSum += currentCount;
                            var layout = new Layout(PangoContext)
                            {
                                Wrap = WrapMode.Word,
                                FontDescription = FontDescription.FromString("Tahoma 10")
                            };
                            layout.SetMarkup(taskCount.ToString());

                            GdkWindow.DrawLayout(taskLabelGc,
                                offset,
                                (int)(deltaActor*(1 - (double)taskCount / maxTaskCout)) + deltaActor * actorIndex,layout);
                        }
                    }

                    offset += delta;
                    labelDate = labelDate.AddDays(1);
                }

                actorIndex++;
            }

            taskLabelGc.Dispose ();

            //DrawActorAxis
            var offsetActor = FBorderMarginV;
            var foregroundColor = new Gdk.Color(0xff, 0, 0);
            var foregroundColor1 = new Gdk.Color(0, 0, 0xff);
            var actorLabelGc = new Gdk.GC (GdkWindow);
            var axisGc = new Gdk.GC (GdkWindow);
            colormap.AllocColor(ref foregroundColor,true,true);
            taskLabelGc.Foreground = foregroundColor;
            axisGc.Foreground = foregroundColor1;

            foreach(DataRow row in Source.Tables["Actor"].Rows)
            {
                var layout = new Layout(PangoContext)
                {
                    Wrap = WrapMode.Word,
                    FontDescription = FontDescription.FromString("Tahoma 10")
                };
                layout.SetMarkup((string)row["Name"]);

                GdkWindow.DrawLayout(actorLabelGc,
                                          FBorderMarginH,
                                          offsetActor - FBorderMarginV,layout);
                GdkWindow.DrawLine(axisGc,
                                        FBorderMarginH,
                                        offsetActor,
                                        fWidth - FBorderMarginH, offsetActor);
                offsetActor += deltaActor;
            }

            axisGc.Dispose ();

            //DrawDateAxis
            var labelDate1 = firstDate;
            var offset1 = FBorderMarginH;
            for (var i = 0; i < deltaSpan.Days; i++)
            {
                var layout = new Layout(PangoContext)
                {
                    Wrap = WrapMode.Word,
                    FontDescription = FontDescription.FromString("Tahoma 10")
                };
                layout.SetMarkup(labelDate1.ToString("dd/MM"));

                GdkWindow.DrawLayout(actorLabelGc,
                                          offset1 + FBorderMarginH,
                                          fHeight -2 * FBorderMarginV - FTaskHeight,
                                          layout);

                GdkWindow.DrawLine(axisGc,
                                        offset1,
                                        FBorderMarginV,
                                        offset1,
                                        fHeight - FBorderMarginV);
                offset1 += delta;
                labelDate1 = labelDate1.AddDays(1);
            }

            actorLabelGc.Dispose ();

            //DrawDateNow
            var nowSpan = DateTime.Now.Subtract(firstDate);

            var offsetDate = FBorderMarginH + (int)(delta*(double)nowSpan.Ticks / TimeSpan.TicksPerDay);

            grw.SetSourceRGB(0, 0, 0);
            grw.MoveTo(offsetDate,
                       FBorderMarginV);
            grw.LineTo (offsetDate,
                        fHeight - FBorderMarginV);
            grw.RelLineTo (new Distance { Dx = -3, Dy = 0 });
            grw.RelLineTo (new Distance { Dx = 3, Dy = -3 });
            grw.RelLineTo (new Distance { Dx = 3, Dy = 3 });
            grw.RelLineTo (new Distance { Dx = -3, Dy = 0 });
            grw.Stroke();

            return true;
        }
    }
}