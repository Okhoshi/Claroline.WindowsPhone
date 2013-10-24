using System;
using System.Windows.Input;

namespace ClarolineApp.RT.Common
{
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CommandEventArgs(string message, object parameter)
        {
            Message = message;
            Parameter = parameter;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        public object Parameter
        {
            get;
            private set;
        }
    }

    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;
        public event EventHandler<CommandEventArgs> Notify;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandBase"/> class.
        /// </summary>
        public CommandBase()
        {
            EventHandler unused = CanExecuteChanged;
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute(object parameter)
        {
            if (Notify != null)
            {
                Notify(this, new CommandEventArgs(this.GetType().ToString(), parameter));
            }
        }
    }

    public class SynchronizeCommand : CommandBase
    {

    }
}
