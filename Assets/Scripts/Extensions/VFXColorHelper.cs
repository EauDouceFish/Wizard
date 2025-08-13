using UnityEngine;

public static class VFXColorHelper
{
    /// <summary>
    /// 批量修改特效颜色（ParticleSystem的StartColor、Color over Lifetime、Trails的Color over Trail），保留原有alpha
    /// </summary>
    public static void ApplyColorToVFX(GameObject root, Color targetColor)
    {
        if (root == null) return;

        var particles = root.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in particles)
        {
            // 修改StartColor
            var main = ps.main;
            Color oldColor = main.startColor.color;
            Color newColor = new Color(targetColor.r, targetColor.g, targetColor.b, oldColor.a);
            main.startColor = newColor;

            // 修改Color over Lifetime
            var col = ps.colorOverLifetime;
            if (col.enabled)
            {
                Gradient grad = col.color.gradient;
                GradientColorKey[] colorKeys = grad.colorKeys;
                GradientAlphaKey[] alphaKeys = grad.alphaKeys;
                if (colorKeys.Length > 0)
                {
                    Color old = colorKeys[0].color;
                    colorKeys[0].color = new Color(targetColor.r, targetColor.g, targetColor.b, old.a);
                    grad.SetKeys(colorKeys, alphaKeys);
                    col.color = new ParticleSystem.MinMaxGradient(grad);
                }
            }

            // 修改Trails的Color over Trail
            var trails = ps.trails;
            if (trails.enabled)
            {
                var minMaxGrad = trails.colorOverTrail;
                Gradient grad = minMaxGrad.gradient;
                GradientColorKey[] colorKeys = grad.colorKeys;
                GradientAlphaKey[] alphaKeys = grad.alphaKeys;
                if (colorKeys.Length > 0)
                {
                    Color old = colorKeys[0].color;
                    colorKeys[0].color = new Color(targetColor.r, targetColor.g, targetColor.b, old.a);
                    grad.SetKeys(colorKeys, alphaKeys);
                    trails.colorOverTrail = new ParticleSystem.MinMaxGradient(grad);
                }
            }
        }
    }
}