# Unity Assets

This Unity project needs its asset pack to run correctly.

Keep Unity source scripts, scenes, prefabs, and `.meta` files in this `Assets` folder so the project can be opened from git. Put private, licensed, or very large assets in:

```bash
unity-plane/Assets/PrivateAssets/
```

Recommended options:

- Add `unity-plane/Assets/PrivateAssets/` as a private git submodule.
- Use Git LFS for large binary assets.
- Keep private download details in an untracked local file named `unityassets.local`.

This project intentionally tracks a public Google Drive link in the root `unityassets` file so collaborators can download the required asset pack.
