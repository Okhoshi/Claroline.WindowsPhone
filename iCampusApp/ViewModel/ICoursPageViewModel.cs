using ClarolineApp.Model;
using ClarolineApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClarolineApp.ViewModel
{
    interface ICoursPageViewModel : IClarolineViewModel
    {
        void OnAnnonceItemSelected(CL_Annonce item);
        void OnDocumentItemSelected(CL_Document item);
        void OnGenericItemSelected(ResourceModel item);

        bool IsOnRoot();
        void GoUp();
        bool IsDocumentPivotSelected(object SelectedItem);
    }
}
