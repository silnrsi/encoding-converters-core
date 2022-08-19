using ECInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BackTranslationHelper
{
    public enum ButtonPressed
    {
        None,
        Copy,
        WriteToTarget,
        MoveToNext,
		Skip,
        Cancel,
        Close
    }

    public interface IBackTranslationHelperDataSource
    {
        Font SourceLanguageFont { get; }
        Font TargetLanguageFont { get; }
        BackTranslationHelperModel Model { get; }
        string ProjectName { get; }

        void ActivateKeyboard();
        void Log(string message);
        void WriteToTarget(string text);
        void MoveToNext();
        void Cancel();

        void SetDataUpdateProc(Action<BackTranslationHelperModel> updateControls);
        void ButtonPressed(ButtonPressed button);
    }
}
