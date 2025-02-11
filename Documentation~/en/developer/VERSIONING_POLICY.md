# Versioning Policy
This project adopts ["Semantic Versioning 2.0.0"](https://semver.org/).

In addition to "Semantic Versioning 2.0.0", following rules applied:
* Each version component is increased by 1 in general release.
* If Major component is 0:
    * Minor component is increased and Patch component reset to 0 if:
        * Public API is changed in Not Backwards Compatible way
    * Patch component is increased if any of following happens:
        * New feature is implemented
        * Bug fix is applied
        * UI changes are made

Those are not counted as Public API:
* Any C# members (class, method, property, field, etc.) that applicable to either:
    * cannot be accessed from outside the codebase without reflection and patching
        * This includes `private`, `internal`, `private protected`, and `file` members.
    * not marked with `[StableAPI]`
* Folder structure.
* Backlink component and its members.

This project may provide experimental feature or implementation.
It is allowed to be disappeared without marking it as Not Backwards Compatible.
