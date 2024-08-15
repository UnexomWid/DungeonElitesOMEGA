using System.Collections;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class FindMissing : MonoBehaviour
{
	private const string MENU_ROOT = "Tools/Missing References/";

	/// <summary>
	/// Finds all missing references to objects in the currently loaded scene.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in scene", false, 50)]
	public static void FindMissingReferencesInCurrentScene()
	{
		var sceneObjects = GetSceneObjects();
		FindMissingReferences(EditorSceneManager.GetActiveScene().path, sceneObjects);
	}

	/// <summary>
	/// Finds all missing references to objects in all enabled scenes in the project.
	/// This works by loading the scenes one by one and checking for missing object references.
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in all scenes", false, 51)]
	public static void FindMissingReferencesInAllScenes()
	{
		foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
		{
			EditorSceneManager.OpenScene(scene.path);
			FindMissingReferencesInCurrentScene();
		}
	}

	/// <summary>
	/// Finds all missing references to objects in assets (objects from the project window).
	/// </summary>
	[MenuItem(MENU_ROOT + "Search in assets", false, 52)]
	public static void FindMissingReferencesInAssets()
	{
		var allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
		var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject).Where(a => a != null).ToArray();

		FindMissingReferences("Project", objs);
	}

	private static void FindMissingReferences(string context, GameObject[] gameObjects)
	{
		Material defaultMaterial = GameObject.Find("DefaultMaterial").GetComponent<SpriteRenderer>().sharedMaterial;

		if (gameObjects == null)
		{
			return;
		}

		foreach (var go in gameObjects)
		{
			var components = go.GetComponents<SpriteRenderer>();

			foreach (var component in components)
			{
				if(component.sharedMaterial == null || component.sharedMaterial
					.name.Contains("Instance"))
                {
					component.sharedMaterial = defaultMaterial;

					Debug.Log("Changed material on " + component.transform.name);
                }
			}
		}

		foreach (var go in gameObjects)
		{
			var components = go.GetComponents<ParticleSystemRenderer>();

			foreach (var component in components)
			{
				if (component.sharedMaterial == null || component.sharedMaterial
					.name.Contains("Instance"))
				{
					component.sharedMaterial = defaultMaterial;

					Debug.Log("Changed material on " + component.transform.name);
				}
			}
		}

		foreach (var go in gameObjects)
		{
			var components = go.GetComponents<Image>();

			foreach (var component in components)
			{
				if (component.material.shader.name.Contains("Error"))
				{
					component.material.shader = defaultMaterial.shader;

					Debug.Log("Changed material on " + component.transform.name);
				}
			}
		}
	}

	private static GameObject[] GetSceneObjects()
	{
		// Use this method since GameObject.FindObjectsOfType will not return disabled objects.
		return Resources.FindObjectsOfTypeAll<GameObject>()
			.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
				   && go.hideFlags == HideFlags.None).ToArray();
	}

	private static void ShowError(string context, GameObject go, string componentName, string propertyName)
	{
		var ERROR_TEMPLATE = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";

		Debug.LogError(string.Format(ERROR_TEMPLATE, GetFullPath(go), componentName, propertyName, context), go);
	}

	private static string GetFullPath(GameObject go)
	{
		return go.transform.parent == null
			? go.name
				: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
	}
}
