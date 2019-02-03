# Caching
Caching consists of both a front- and a backend. The frontend determines how you may interact with the cache, i.e. specifies an interface on how data is accepted and will be returned. The backend handles how the data is actually stored.

## Backends
Backends implement the `ICacheBackend` interface, thus providing means to `Get`, `Set`, `Remove` keys and check for their existence with `Has`.
Depending on their implementation and capabilities they may have additional options such as a `timeToLive` for the stored values and so on.

An example would be a `FileBackend` storing (long-term) cache-entries in the file system or a `DatabaseBackend` storing values in a database.

## Frontends
Frontends implement the `ICacheFrontend` interface of extend the `AbstractCacheFrontend` and provide basically the same actions as the backend does.
However while some frontends may just invoke their backend's respective interface, they may e.g. first serialize (or unserialize) the data they should hold.

A cache will most likely be created by constructing a frontend with a backend, e.g.
```cs
var cache = new StringFrontend("identifier", new FileBackend(...));
```