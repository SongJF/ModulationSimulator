using MaterialDesignThemes.Wpf;
using ModulationSimulator.Pages;
using System;

namespace ModulationSimulator.Domain
{
    class MainWindowViewModel
    {
        public MenuList[] MenuLists { get; }

        public MainWindowViewModel(ISnackbarMessageQueue snackbarMessageQueue)
        {
            if (snackbarMessageQueue == null) throw new ArgumentNullException(nameof(snackbarMessageQueue));

            MenuLists = new[]
            {
                new MenuList(Properties.Resources.MenuName_Hello,new Hello()),
                new MenuList(Properties.Resources.MenuName_Spectrum,new Exp_Spectrum()),
                new MenuList(Properties.Resources.MenuName_AM,new Exp_AM())
            };
        }
    }
}
