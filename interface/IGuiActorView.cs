//author:Eugene Pirogov
//email:eugene.intalk@gmail.com
//license:GPLv3.0
//date:4/12/2014
// created on 30.12.2005 at 20:55
namespace TaskManagerInterface
{
    public interface IGuiActorView : IGuiMessageDialog
    {
        string ActorEmail
        {
            get;
            set;
        }

        string ActorName
        {
            get;
            set;
        }
    }
}