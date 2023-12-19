// An event handler that passes its arguments by readonly reference.
public delegate void RefEventHandler<TEventArgs>(object sender, in TEventArgs e);