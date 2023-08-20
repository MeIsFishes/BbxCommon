UI items in BbxCommon can be directly added to GameObjects in Unity. We aim to implement UI functions mainly in the library, and keep it low-code in gameplay as possible.
An ideal workflow is like this:
  - The most functions especially those graphic-related are implemented in the library and settings are integrated on Unity's Inspector to offer to customize.
  - As a gameplay developer, you can simply take cares of only Wrapper in each item, which offers interfaces for developers to pass datas and set callbacks.
  - Sure there must be some functions or styles we haven't support. We advise not to design such styles, for UI functions always take us much time but we consider it is not important for an indie game.
  - If there is an important function we haven't suppoted, it's best to implement and add it into the library.