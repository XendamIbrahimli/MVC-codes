
using UniqloMVC.ViewModel.Product;
using UniqloMVC.ViewModel.Slider;

namespace UniqloMVC.ViewModel.Common
{
    public class HomeVM
    {
        public IEnumerable<SliderItemVM> Sliders { get; set; }
        public IEnumerable<ProductItemVM> Products { get; set; }
    }
}
