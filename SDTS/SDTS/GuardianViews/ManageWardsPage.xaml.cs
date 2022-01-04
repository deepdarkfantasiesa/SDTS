﻿using SDTS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SDTS.GuardianViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageWardsPage : ContentPage
    {
        ManageWardsViewModel _viewModel;
        public ManageWardsPage()
        {
            InitializeComponent();
            BindingContext = _viewModel = new ManageWardsViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}