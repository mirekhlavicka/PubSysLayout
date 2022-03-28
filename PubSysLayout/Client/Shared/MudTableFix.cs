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
        public override TableContext TableContext
        {
            get
            {
                if (ContextFix == null)
                {
                    ContextFix = new TableContextFix<T>();
                    _getBackingField(this, "Context").SetValue(this, ContextFix);
                }

                Context.Table = this;
                Context.TableStateHasChanged = this.StateHasChanged;

                return ContextFix;
            }
        }

        private TableContextFix<T> ContextFix  = null; // new TableContextFix<T>();

        private string _getBackingFieldName(string propertyName)
        {
            return string.Format("<{0}>k__BackingField", propertyName);
        }

        private FieldInfo _getBackingField(object obj, string propertyName)
        {

            return obj.GetType().BaseType.GetField(_getBackingFieldName(propertyName), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        }
    }

    public class TableContextFix<T> : TableContext<T>
    {        
        public override void Remove(MudTr row, object item) {  }
    }
}