using HonooUI.WPF;

namespace Honoo.MangaPacker.Models

{
    internal static class ModelLocator
    {
        internal static DialogLocale DialogLocaleCHS { get; } = new DialogLocale() { OKText = "确 定", CancelText = "取 消", YesText = "是", NoText = "否" };
        internal static DialogOptions DialogOptionsAuto { get; } = new DialogOptions() { Locale = DialogLocaleCHS };
        internal static Settings Settings { get; } = new Settings();
    }
}