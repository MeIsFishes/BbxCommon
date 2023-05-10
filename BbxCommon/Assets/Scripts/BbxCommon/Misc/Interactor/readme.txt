Interactor signs a GameObject interactive with other ones.
An Interactor object should be with some interacting flags, users need to register valid interacting requests to InteractorManager. Once an Interactor awake or request
to interact with another one, it will invoke its callbacks.