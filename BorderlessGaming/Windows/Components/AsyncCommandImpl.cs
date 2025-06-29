#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BorderlessGaming.Windows.Components;
internal class AsyncCommandImpl : ICommand {
  /// <summary>
  /// The internal command to execute.
  /// </summary>
  private readonly Predicate<object?> _canExecute;

  /// <summary>
  /// The internal command to test if we can execute.
  /// </summary>
  private readonly Func<Task> _execute;

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandImpl"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  internal AsyncCommandImpl(Func<Task> execute) : this(execute, _ => true) { }

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandImpl"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  /// <param name="canExecute">The can execute</param>
  private AsyncCommandImpl(Func<Task> execute, Predicate<object?> canExecute) {
    this._execute = execute;
    this._canExecute = canExecute;
  }

  /// <summary>
  /// Tests if the method can be executed with the given parameter.
  /// </summary>
  /// <param name="parameter">The parameter</param>
  /// <returns><see langword="true" /> if this command can execute; otherwise <see langword="false" />.</returns>
  public bool CanExecute(object? parameter)
    => this._canExecute(parameter);

  /// <summary>
  /// The event handler when the indicator of this command's execution requirements change.
  /// </summary>
  public event EventHandler? CanExecuteChanged {
    add => CommandManager.RequerySuggested += value;
    remove => CommandManager.RequerySuggested -= value;
  }

  /// <summary>
  /// Executes the command with the given parameter
  /// </summary>
  /// <param name="parameter">The parameter</param>
  public void Execute(object? parameter) {
    this.ExecuteAsync(parameter);
  }

  public Task ExecuteAsync(object? parameter)
    => this._execute();

  /// <summary>
  /// Refreshes this instance
  /// </summary>
  internal void Refresh()
    => CommandManager.InvalidateRequerySuggested();
}
