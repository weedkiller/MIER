﻿using Xamarin.Forms.Internals;

namespace DDM.Behaviors
{
    [Preserve(AllMembers = true)]
    public interface IAction
    {
        bool Execute(object sender, object parameter);
    }
}