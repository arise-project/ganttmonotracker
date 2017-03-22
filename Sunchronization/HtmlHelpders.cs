﻿using System;
using System.Text;
using System.Data;
namespace GanttMonoTracker
{
	public static class HtmlHelpders
	{
		public static string ToHTMLTable(DataTable dt)
		{
			if (dt.Rows.Count == 0) return ""; // enter code here

			StringBuilder builder = new StringBuilder();
			builder.Append("<html>");
			builder.Append("<head>");
			builder.Append("<title>");
			builder.Append("Page-");
			builder.Append(Guid.NewGuid());
			builder.Append("</title>");
			builder.Append("</head>");
			builder.Append("<body>");
			builder.Append("<table border='1px' cellpadding='5' cellspacing='0' ");
			builder.Append("style='border: solid 1px Silver; font-size: x-small;'>");
			builder.Append("<tr align='left' valign='top'>");
			foreach (DataColumn c in dt.Columns)
			{
				builder.Append("<td align='left' valign='top'><b>");
				builder.Append(c.ColumnName);
				builder.Append("</b></td>");
			}
			builder.Append("</tr>");
			foreach (DataRow r in dt.Rows)
			{
				builder.Append("<tr align='left' valign='top'>");
				foreach (DataColumn c in dt.Columns)
				{
					builder.Append("<td align='left' valign='top'>");
					builder.Append(r[c.ColumnName]);
					builder.Append("</td>");
				}
				builder.Append("</tr>");
			}
			builder.Append("</table>");
			builder.Append("</body>");
			builder.Append("</html>");

			return string.Format(@"<HEAD>\
<TITLE>Prolucid</TITLE>\
<meta charset='utf-8'/>
</HEAD>\
<BODY BGCOLOR = 'WHITE' >\
< CENTER >\
< H1 > A Simple Sample Web Page</H1>
{0}
			</BODY>",  builder.ToString());
		}
	}
}
