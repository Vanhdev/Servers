using System;
using System.Collections.Generic;
using System.Linq;
using Vst.GUI;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Models;

namespace Monitoring.Views
{
    public  class PageDataView : PageDataView<object>
    {

    }
    public class PageDataView<T> : BaseView<PageViewLayout, T>
    {

        static PageViewLayout _layout;
        protected TableInfo _tableInfo;
        protected override PageViewLayout CreateMainContent()
        {
            if (_layout == null)
            {
                _layout = new PageViewLayout();
            }
            return _layout;
        }
        protected virtual void CreatePagesCore(object items)
        {
            MainContent.CreatePages((System.Collections.IEnumerable)items);
        }
        protected virtual void SetTableLayout(Border parent, UIElement layout)
        {
            parent.Child = layout;
        }
        protected virtual string GetTableTemplateName() => ControllerName;
        protected virtual void LoadTemplate()
        {
            _table.LoadTemplate(_tableInfo.Columns);
        }
        protected override MenuInfoList GetActions()
        {
            return new MenuInfoList {
                new MenuInfo {
                    Text = "Actions",
                    Childs = {
                        new MenuInfo("Refresh", "Ctrl+R", null, RunRefresh),
                        new MenuInfo("Copy", "Ctrl+C", null, RunCopy) { BeginGroup = true },
                        new MenuInfo("Paste", "Ctrl+V", null, RunPaste),
                        new MenuInfo("New", "Ctrl+N", null, RunAddNew) { BeginGroup = true },
                        new MenuInfo("Delete", "Ctrl+X", null, RunDeleteSelected) { BeginGroup = true },
                        //new MenuInfo("Delete All", "Ctrl+Shift+X", null, RunDeleteAll),
                    }
                },
            };
        }
        public Action RunCopy { get; set; }
        public Action RunDeleteSelected { get; set; }
        public virtual void RunDeleteAll()
        {
            if (DisplayConfirm("Chắc chắn xóa toàn bộ dữ liệu?") == true)
            {
                MainContent.Request(ControllerName + "/clear");
            }
        }
        public virtual void RunRefresh()
        {
            _table.Dispatcher.InvokeAsync(() => _table.Refresh());
        }
        public Action RunPaste { get; set; }
        public Action RunAddNew { get; set; }

        public override string MainCaption => _tableInfo.Caption;

        protected TemplateDataTable _table;

        protected void CallSelectedItems(Action<EditingContext> callback)
        {
            var items = _table.SelectedItems;
            if (items.Length == 0)
            {
                DisplayAlert("Hãy chọn ít nhất một bản ghi.");
                return;
            }

            var context = new EditingContext
            {
                Value = new DataContext()
            };
            foreach (DataContext item in items)
            {
                context.Value.Add(item.ObjectId, item);
            }
            callback.Invoke(context);
        }

        protected override void RenderCore()
        {
            var layout = new TemplateDataTableLayout();
            _table = layout.Table;
            _tableInfo = GUI.Tables[GetTableTemplateName()];

            LoadTemplate();

            RunCopy = () => {
                Clipboard.SetText(_table.Copy());
                DisplayAlert("Đã copy dữ liệu vào clipboard.");

            };
            RunPaste = () => {
                MainContent.Request("Import/" + ControllerName);
            };
            RunAddNew = () => MainContent.Request(ControllerName + "/add");
            RunDeleteSelected = () => {
                CallSelectedItems(itemsContext =>
                {
                    if (DisplayConfirm("Chắc chắn xóa các bản ghi đã chọn?") == true)
                    {
                        itemsContext.Action = EditingActions.Delete;
                        MainContent.Request(ControllerName + "/Delete", itemsContext);

                        foreach (var item in itemsContext.Value.Values)
                        {
                            MainContent.CurrentPage.Remove(item);
                        }
                        _table.RemoveSelectedRows();
                    }
                });
            };

            MainContent.ListContent.Child = layout;
            MainContent.CurrentPageChanged += (s, e) => {
                _table.ItemsSource = MainContent.CurrentPage;
            };

            CreatePagesCore(Model);
            SetTableLayout(MainContent.ListContent, layout);
            //foreach (var p in MainContent.Pages)

            //table.ActiveRowStyle.Color = "#ccc";

            _table.OpenItem = (s) => {
                RaiseItemSelected(s);
            };
            _table.ItemFocused = (s) => RaiseItemFocused(s);


        }

        protected override void CreateActions()
        {
            base.CreateActions();
            CreateMenu(MainContent.MenuContent, GetActions());

            SetHotKey(MainContent.PageSelector.Children);
            SetHotKey(MainContent.PageSizeOption.Items);
        }
        protected virtual void RaiseItemSelected(object item)
        {
            MainContent.Request(ControllerName + "/edit", ((DataContext)item).ObjectId);
        }
        protected virtual void RaiseItemFocused(object item)
        {
        }
    }
}
