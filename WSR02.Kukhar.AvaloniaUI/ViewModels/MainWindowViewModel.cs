using Avalonia.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSR02.Kukhar.AvaloniaUI.Models;
using WSR02.Kukhar.AvaloniaUI.Models.NotDbModels;
using WSR02.Kukhar.AvaloniaUI.Views;

namespace WSR02.Kukhar.AvaloniaUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
		private List<Plate> _items = new();
		private string _search = null!;
		private List<string> _sortingList = new();
        private List<string> _filteringList = new();
        private string _selectedSorting = null!;
        private string _selectedFiltering = null!;


        #region Propties

        public List<Plate> Items
		{
			get { return _items; }
			set 
			{ 
				_items = value;
				OnPropertyChanged(nameof(Items));
			}
		}

        public string Search
        {
            get => _search;
            set
            {
                _search = value;
                GetSearchingBase(value);
                OnPropertyChanged(nameof(Search));
            }
        }

        public List<string> SortingList
        {
            get { return _sortingList; }
            set { _sortingList = value; }
        }

        public List<string> FilteringList
        {
            get { return _filteringList; }
            set { _filteringList = value; }
        }

        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                _selectedSorting = value;
                if (value != "")
                    GetSortingBase(Items, value);
                OnPropertyChanged(nameof(SelectedSorting));
            }
        }

        public string SelectedFiltering
        {
            get => _selectedFiltering;
            set
            {
                _selectedFiltering = value;
                if (value != "")
                    GetFilteringBase(value);
                OnPropertyChanged(nameof(SelectedFiltering));
            }
        }

        #endregion


        public MainWindowViewModel()
        {
            Items = GetDatabaseItems();
            CreateSortingAndFilteringList();
        }


        #region DataMakingMethods

        private List<Plate> GetDatabaseItems()
        {
            List<Plate> items = new();

            var products = new WSR02KukharContext()
                .Products
                .Include(p => p.ProductType)
                .ToList();

            foreach (var product in products)
            {
                string? materials = GetMaterials(product.Id);
                decimal cost = GetCost(product.Id);
                var image = GetImage(product.Image);
                var plate = new Plate(product.Title, product.ProductType.Title,
                    product.ArticleNumber, cost, materials, image);

                items.Add(plate);
            }

            return items;
        }

        private string? GetMaterials(int productId)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var productMaterials = new WSR02KukharContext()
                 .ProductMaterials
                 .Include(m => m.Material)
                 .Where(p => p.ProductId == productId)
                 .ToList();

            if (productMaterials.Count == 0)
                return null;

            stringBuilder.Append("Материалы: ");

            foreach (var material in productMaterials)
                stringBuilder.Append($"{material.Material.Title}, ");

            stringBuilder.Remove(stringBuilder.Length - 2, 2);
            return stringBuilder.ToString();              
        }

        private decimal GetCost(int productId)
        {
            var productMaterials = new WSR02KukharContext()
                 .ProductMaterials
                 .Include(m => m.Material)
                 .Where(p => p.ProductId == productId)
                 .ToList();

            decimal cost = 0;
            foreach (var productMaterial in productMaterials)
            {
                cost += Convert.ToDecimal(productMaterial.Count) * productMaterial.Material.Cost;
            }
            return cost;

        }

        private Bitmap GetImage(string image)
        {
            if (image == "")
                image = @"\Assets\picture.png";
            return new Bitmap("." + image);
        }

        #endregion


        #region SearchingFilteringSortingMakingMethods

        private void GetFilteringBase(string variable)
        {
            if (variable == "Без фильтрации" || variable == null)
            {
                SelectedFiltering = "";
                Items = GetDatabaseItems();
            }
            else if (variable == "С материалами")
            {
                Items = GetDatabaseItems()
                    .Where(f => f.Materials != null)
                    .ToList();
            }
            else if (variable == "Без материалов")
            {
                Items = GetDatabaseItems()
                    .Where(f => f.Materials == null)
                    .ToList();
            }
            else
            {
                Items = GetDatabaseItems()
                    .Where(f => f.Type == variable)
                    .ToList();
            }
            GetSortingBase(Items, SelectedSorting);
        }

        private void GetSortingBase(List<Plate> items, string variable)
        {
            if (variable == "Без сортировки")
            {
                SelectedSorting = "";
                Items = GetDatabaseItems()
                    .Where(item => items.Any(t => t.Title == item.Title))
                    .ToList();
            }
            else if(variable == "По названию")
            {
                Items = items.OrderBy(t => t.Title).ToList();
            }
            else if (variable == "По типу")
            {
                Items = items.OrderBy(t => t.Type).ToList();
            }
            else if (variable == "По стоимости")
            {
                Items = items.OrderBy(t => t.Cost).ToList();
            }
            else if (variable == "По артиклу")
            {
                Items = items.OrderBy(t => t.ArticleNumber).ToList();
            }
        }

        private void GetSearchingBase(string variable)
        {
            if (Items.Count() == 0)
            {
                GetFilteringBase(SelectedFiltering);
            }
            Items = Items
                .Where(s => s.Title.ToLower().Contains(variable.ToLower()))
                .ToList();
            GetSortingBase(Items, SelectedSorting);
        }

        private void CreateSortingAndFilteringList()
        {
            SortingList.Add("Без сортировки");
            SortingList.Add("По названию");
            SortingList.Add("По типу");
            SortingList.Add("По стоимости");
            SortingList.Add("По артиклу");

            FilteringList.Add("Без фильтрации");
            FilteringList.Add("С материалами");
            FilteringList.Add("Без материалов");
            foreach (var pt in new WSR02KukharContext().ProductTypes)
            {
                FilteringList.Add(pt.Title);
            }
        }

        #endregion
    }
}
