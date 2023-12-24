using UnityEngine;
using UnityEngine.Pool;

namespace LOONACIA.Unity.Pool
{
	public class Poolable : MonoBehaviour
	{
		public IObjectPool<Poolable> Pool { get; set; }
	}
}