using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace SampleApp
{
    public class ButtonCommand:ICommand
    {
        private Action execute;

        public ButtonCommand(Action execute)
        {
            this.execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            execute();
        }
    }
}
