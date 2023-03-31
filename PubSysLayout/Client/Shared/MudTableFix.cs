using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class MudTableFix<T> : MudTable<T>
    {
        //public MudTableFix()
        //{
        //    this.GetType().BaseType
        //        .GetField("<Context>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic )
        //        .SetValue(this, new TableContextFix<T>());
        //}

        public bool StopRender = false;

        protected override bool ShouldRender()
        {
            if (StopRender)
            {
                return false;
            }
            else
            { 
                return base.ShouldRender();
            }
        }
    }

    //public class TableContextFix<T> : TableContext<T>
    //{        
    //    public override void Remove(MudTr row, object item) 
    //    {
    //        if (this.Table != null && this.Table is MudTable<T> && !(this.Table as MudTable<T>).Items.Contains(item.As<T>()))
    //        {
    //            base.Remove(row, item);
    //        }
    //    }
    //}
}