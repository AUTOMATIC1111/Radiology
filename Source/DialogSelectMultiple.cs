using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Radiology
{
    interface ISelectMultiple<T>
    {
        IEnumerable<T> All();

        string Label(T obj);
        bool IsSelected(T obj);
        void Select(T obj);
        void Unselect(T obj);
    }

    class DialogSelectMultiple<T> : Window
    {
        ISelectMultiple<T> items;

        public string LabelSelected { get; set; }
        public string LabelNotSelected { get; set; }
        public int SelectedLimit { get; set; } = -1;

        public DialogSelectMultiple(ISelectMultiple<T> items)
        {
            this.items = items;

            doCloseButton = true;
            doCloseX = true;
            closeOnClickedOutside = true;
            absorbInputAroundWindow = true;
        }

        public override Vector2 InitialSize => new Vector2(620f, 500f);
        
        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Rect outRect = new Rect(inRect);
            outRect.yMin += 20f;
            outRect.yMax -= 40f;
            outRect.width -= 16f;
            Rect viewRect = new Rect(0f, 0f, outRect.width - 16f, items.All().Count() * 35f + 100f);
            Widgets.BeginScrollView(outRect, ref this.scrollPosition, viewRect, true);
            try
            {
                float num = 0f;
                bool flag = false;
                int countSelected = 0;
                IEnumerable<T> selected = items.All().Where(x => items.IsSelected(x));

                foreach (T item in selected)
                {
                    flag = true;
                    Rect rect = new Rect(0f, num, viewRect.width * 0.6f, 32f);
                    Widgets.Label(rect, items.Label(item));
                    rect.x = rect.xMax;
                    rect.width = viewRect.width * 0.4f;
                    if (Widgets.ButtonText(rect, LabelSelected.Translate(), true, false, true))
                    {
                        items.Unselect(item);
                        RimWorld.SoundDefOf.Click.PlayOneShotOnCamera(null);
                        return;
                    }
                    num += 35f;
                    countSelected++;
                }
                if (flag)
                {
                    num += 15f;
                }

                bool active = SelectedLimit == -1 || countSelected < SelectedLimit;

                foreach (T item in items.All().Except(selected))
                {
                    Rect rect2 = new Rect(0f, num, viewRect.width * 0.6f, 32f);
                    Widgets.Label(rect2, items.Label(item));
                    rect2.x = rect2.xMax;
                    rect2.width = viewRect.width * 0.4f;
                    string label = LabelNotSelected.Translate();
                    if (active && Widgets.ButtonText(rect2, label, true, false, true))
                    {
                        items.Select(item);
                        RimWorld.SoundDefOf.Click.PlayOneShotOnCamera(null);
                        break;
                    }
                    num += 35f;
                }
            }
            finally
            {
                Widgets.EndScrollView();
            }
        }
        
        private Vector2 scrollPosition;

        private const float EntryHeight = 35f;
    }
}
