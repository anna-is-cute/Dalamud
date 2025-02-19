﻿using System.Numerics;

using ImGuiScene;

namespace Dalamud.Interface.Internal;

/// <summary>
/// Base TextureWrap interface for all Dalamud-owned texture wraps.
/// Used to avoid referencing ImGuiScene.
/// </summary>
public interface IDalamudTextureWrap : IDisposable
{
    /// <summary>
    /// Gets a texture handle suitable for direct use with ImGui functions.
    /// </summary>
    IntPtr ImGuiHandle { get; }

    /// <summary>
    /// Gets the width of the texture.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Gets the height of the texture.
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Gets the size vector of the texture using Width, Height.
    /// </summary>
    Vector2 Size => new(this.Width, this.Height);
}

/// <summary>
/// Safety harness for ImGuiScene textures that will defer destruction until
/// the end of the frame.
/// </summary>
public class DalamudTextureWrap : IDalamudTextureWrap
{
    private readonly TextureWrap wrappedWrap;

    /// <summary>
    /// Initializes a new instance of the <see cref="DalamudTextureWrap"/> class.
    /// </summary>
    /// <param name="wrappingWrap">The texture wrap to wrap.</param>
    internal DalamudTextureWrap(TextureWrap wrappingWrap)
    {
        this.wrappedWrap = wrappingWrap;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DalamudTextureWrap"/> class.
    /// </summary>
    ~DalamudTextureWrap()
    {
        this.Dispose(false);
    }

    /// <summary>
    /// Gets the ImGui handle of the texture.
    /// </summary>
    public IntPtr ImGuiHandle => this.wrappedWrap.ImGuiHandle;

    /// <summary>
    /// Gets the width of the texture.
    /// </summary>
    public int Width => this.wrappedWrap.Width;

    /// <summary>
    /// Gets the height of the texture.
    /// </summary>
    public int Height => this.wrappedWrap.Height;

    /// <summary>
    /// Queue the texture to be disposed once the frame ends.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Actually dispose the wrapped texture.
    /// </summary>
    internal void RealDispose()
    {
        this.wrappedWrap.Dispose();
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Service<InterfaceManager>.GetNullable()?.EnqueueDeferredDispose(this);
        }
    }
}
