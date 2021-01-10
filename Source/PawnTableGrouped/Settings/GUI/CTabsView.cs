using RWLayout.alpha2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PawnTableGrouped
{
    class CTabPage : CElement
    {
        public virtual string Title { get; set; }

    }

    class CTabsView : CElement
    {
        List<CTabPage> tabs = new List<CTabPage>();
        CElement buttonsPanel;
        CElement tabsHost;
        List<CButtonText> buttons = new List<CButtonText>();

        int tabIndex = 0;
        public int TabIndex
        {
            get => tabIndex;
            set
            {
                tabIndex = value;
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabs[i].Hidden = tabIndex != i;
                }
            }
        }

        public CTabsView()
        {
            buttonsPanel = AddElement(new CElement());
            tabsHost = AddElement(new CElement());

            this.StackTop((buttonsPanel, 30), tabsHost);

        }

        public T AddTab<T>(T tab) where T : CTabPage
        {
            var tabIndex = tabs.Count;
            var anchor = buttons.Count == 0 ? buttonsPanel.left : buttons.Last().right;
            tabs.Add(tab);
            var button = new CButtonText
            {
                Title = tab.Title,
                Action = (_) =>
                {
                    TabIndex = tabIndex;
                }
            };

            buttons.Add(button);
            buttonsPanel.AddElement(button);

            buttonsPanel.AddConstraints(button.top ^ buttonsPanel.top, button.bottom ^ buttonsPanel.bottom, button.left ^ anchor, button.width ^ 150);

            tabsHost.AddElement(tab);
            tabsHost.Embed(tab);
            tab.Hidden = TabIndex != tabIndex;

            return tab;
        }


    }

}
