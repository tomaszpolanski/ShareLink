using System.Windows.Input;

namespace ShareLink.DesignViewModels
{
    public class MainPageDesignViewModel
    {
        public object Text
        {
            get { return new {Value = "www.test.com"}; }
        }

        public object ErrorMessage
        {
            get { return new { Value = "" }; }
        }

        public object IsInProgress
        {
            get { return new { Value = false }; }
        }

        public object SelectAllTextTrigger
        {
            get { return null; }
        }

        public ICommand KeyPressedCommand
        {
            get { return null; }
        }

        public ICommand ShareCommand
        {
            get { return null; }
        }
    }
}