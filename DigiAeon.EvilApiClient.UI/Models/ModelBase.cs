using DigiAeon.EvilApiClient.UI.Bootstrapper.Interfaces;
using DigiAeon.EvilApiClient.UI.ViewModels;

namespace DigiAeon.EvilApiClient.UI.Models
{
    public abstract class ModelBase<T> where T : ViewModelBase, new()
    {
        protected ModelBase(T viewModel, IConfig config)
        {
            ViewModel = viewModel;
            Config = config;
        }
        public T ViewModel { get; set; }
        public IConfig Config { get; }
    }
}