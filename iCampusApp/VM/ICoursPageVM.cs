﻿using ClarolineApp.Model;
using ClarolineApp.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClarolineApp.VM
{
    interface ICoursPageVM : IClarolineVM
    {
        void OnItemWithDetailsSelected(CL_Annonce item);
        void OnDocumentItemSelected(CL_Document item);
        void OnGenericItemSelected(ResourceModel item);

        bool IsOnRoot();
        void GoUp();
        bool IsPivotSelectedOfType(object selectedItem, Type type);
        bool IsModuleVisible(object module);
    }
}
