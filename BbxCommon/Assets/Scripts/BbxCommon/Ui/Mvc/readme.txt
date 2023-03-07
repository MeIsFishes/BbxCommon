This is a UI framework which we call it MVC, includes UiModel, UiView, UiController and additonally UiScene for BbxCommon version.
  - UiModel: A database for UI system. Datas those are taken cares of can be stored in UiModel, UiController can read data from it and listen if data changes.
  - UiView: UiView is for user to relate items to UiController and comstomize settings on Inspector. Each UI prefab must have a UiView on root.
  - UiController: Gets data from UiModel or gameplay, process events, and finally operates items on UI via UiView bases on the data.
  - UiScene: A container and manager of UI objects. You can hang UI objects on different UI group in UiScene,  get UiController and do many other things via UiScene.
       A tool UiSceneExporter can help you to export UI objects' data to a ScriptableObject, and UiScene can automatically create those objects by loading exported data.

You are not forced to use the whole MVC framework if you are working on a lightweight project. For example, you can use V&C with no model to create UI objects, and
create objects manually in UiScene to help you to reduce code lines.