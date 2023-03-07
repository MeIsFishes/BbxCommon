ObjectPool only support classes which inherited from PooledObject, which is with OnAllocate() and OnCollect() callbacks.
SimplePool can allocate any types you want, but it cannot initialize or deep free the objects through callbacks.

When pooled objects are past, we cannot know if it is collected, so in most cases, it is recommended to pass pooled object by PooledObjRef<>.
PooledObjRef holds pooled object's id to check if it is collected before returning the value.