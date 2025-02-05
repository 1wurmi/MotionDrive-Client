using Avalonia.Media;
using MsBox.Avalonia.ViewModels.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace MotionDrive.Desktop.Views.Popup;
public class PopupViewModel : ReactiveObject
{
    private string _title;
    private string _description;
    public string Title { get => this._title; set => this.RaiseAndSetIfChanged(ref this._title, value); }
    public string Description { get => this._description; set => this.RaiseAndSetIfChanged(ref this._description, value); }

    public ReactiveCommand<Unit, Unit> OnClosePopup { get; }

    public ObservableCollection<PopupButton> ButtonList { get; set; }

    public PopupViewModel(string title, string description, ObservableCollection<PopupButton> buttonList, Action onClosePopup)
    {
        this.Title = title;
        this.Description = description;
        this.OnClosePopup = ReactiveCommand.Create(onClosePopup);

        this.ButtonList = buttonList;
    }
}

public class PopupButton
{
    public string Name { get; set; }
    public SolidColorBrush ButtonBackground { get; set; }
    public ReactiveCommand<Unit, Unit> OnClick { get; }

    public PopupButton(string name, SolidColorBrush buttonBackground, Action onClick)
    {
        Name = name;
        this.ButtonBackground = buttonBackground;
        OnClick = ReactiveCommand.Create(onClick);
    }
}