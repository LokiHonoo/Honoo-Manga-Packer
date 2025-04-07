using HonooUI.WPF;

namespace Honoo.MangaPacker.Models

{
    internal static class ModelLocator
    {
        internal static DialogOptions DialogOptionsAuto { get; } = new DialogOptions() { Localization = new DialogLocalization("确 定", "取 消", "是", "否") };
        internal static Settings Settings { get; } = new Settings();
    }
}