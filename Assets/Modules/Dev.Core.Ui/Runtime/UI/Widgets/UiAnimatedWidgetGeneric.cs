using System;
using Dev.Core.Ui.UI.Panels.Data;

namespace Dev.Core.Ui.UI.Widgets
{
    public class UiAnimatedWidget<T> : UiAnimatedWidget
        where T : UIPanelData
    {
        protected T Data { get; private set; }

        public override Type RequiredPanelDataType => typeof(T);

        public override void ShowWidget(UIPanelData data = null, bool immediate = false)
        {
            base.ShowWidget(data, immediate);
            Data = (T)data;
        }
    }
}