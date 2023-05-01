using System;
using System.Collections.Generic;
using Bearded.UI.Controls;
using Bearded.Utilities;
using Void = Bearded.Utilities.Void;

namespace Bearded.UI.Navigation
{
    public sealed class NavigationController
    {
        private readonly IControlParent root;
        private readonly DependencyResolver dependencyResolver;
        private readonly IDictionary<Type, object> modelFactories;
        private readonly IDictionary<Type, object> viewFactories;
        private readonly IDictionary<INavigationNode, Control> viewsByModel = new Dictionary<INavigationNode, Control>();

        public event VoidEventHandler? Exited;

        public NavigationController(
            IControlParent root,
            DependencyResolver dependencyResolver,
            IDictionary<Type, object> modelFactories,
            IDictionary<Type, object> viewFactories)
        {
            this.root = root;
            this.dependencyResolver = dependencyResolver;
            this.modelFactories = modelFactories;
            this.viewFactories = viewFactories;
        }

        public void Exit()
        {
            CloseAll();
            Exited?.Invoke();
        }

        public void CloseAll()
        {
            foreach (var (node, view) in viewsByModel)
            {
                node.Terminate();
                root.Remove(view);
            }
            viewsByModel.Clear();
        }

        public void Close(INavigationNode toClose)
        {
            var viewToRemove = viewsByModel[toClose];
            toClose.Terminate();
            root.Remove(viewToRemove);
            viewsByModel.Remove(toClose);
        }

        public void ReplaceAll<TModel>()
            where TModel : NavigationNode<Void>
        {
            ReplaceAll<TModel, Void>(default);
        }

        public void ReplaceAll<TModel, TParameters>(TParameters parameters)
            where TModel : NavigationNode<TParameters>
        {
            CloseAll();
            Push<TModel, TParameters>(parameters);
        }

        public void Replace<TModel>(INavigationNode toReplace)
            where TModel : NavigationNode<Void>
        {
            Replace<TModel, Void>(default, toReplace);
        }

        public void Replace<TModel, TParameters>(TParameters parameters, INavigationNode toReplace)
            where TModel : NavigationNode<TParameters>
        {
            var viewToReplace = viewsByModel[toReplace];
            toReplace.Terminate();
            var (_, view) = instantiateModelAndView<TModel, TParameters>(parameters);
            new AnchorTemplate(viewToReplace).ApplyTo(view);
            root.AddOnTopOf(viewToReplace, view);
            root.Remove(viewToReplace);
            viewsByModel.Remove(toReplace);
        }

        public TModel Push<TModel>()
            where TModel : NavigationNode<Void>
        {
            return Push<TModel, Void>(default);
        }

        public TModel Push<TModel>(Func<AnchorTemplate, AnchorTemplate> build)
            where TModel : NavigationNode<Void>
        {
            return Push<TModel, Void>(default, build);
        }

        public TModel Push<TModel, TParameters>(TParameters parameters)
            where TModel : NavigationNode<TParameters>
        {
            return Push<TModel, TParameters>(parameters, a => a);
        }

        public TModel Push<TModel, TParameters>(TParameters parameters, Func<AnchorTemplate, AnchorTemplate> build)
            where TModel : NavigationNode<TParameters>
        {
            var (model, view) = instantiateModelAndView<TModel, TParameters>(parameters);
            view.Anchor(build);
            root.Add(view);
            return model;
        }

        private (TModel model, Control view) instantiateModelAndView<TModel, TParameters>(TParameters parameters)
            where TModel : NavigationNode<TParameters>
        {
            var model = findModelFactory<TModel>()();
            model.Initialize(createNavigationContext(parameters));
            var view = findViewFactory<TModel>()(model);
            viewsByModel.Add(model, view);

            return (model, view);
        }

        private Func<T> findModelFactory<T>() => (Func<T>) modelFactories[typeof(T)];

        private Func<T, Control> findViewFactory<T>() => (Func<T, Control>) viewFactories[typeof(T)];

        private NavigationContext<T> createNavigationContext<T>(T parameters) =>
            new(this, dependencyResolver, parameters);
    }
}
