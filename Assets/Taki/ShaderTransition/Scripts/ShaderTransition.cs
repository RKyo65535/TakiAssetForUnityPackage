using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taki.TakiTransition
{
    public class ShaderTransition : MonoBehaviour
    {

        Material material;//トランジションマテリアルを格納する変数
        float str;//変化の度合い。0～1の値をとり、その大きさでシェーダーの効力を変える。


        //enumで状態を定義。
        //最初呼び出されたり、TransitionOpen()をしている間はOpen
        //TransitionCloce()をするとcloseになる。
        public enum Mode
        {
            open,
            close
        }
        Mode mode;

        //何Frameかけてトランジションを完結させるかを指定してもらう
        [SerializeField] int flame;

        //シーン遷移を非同期で行うため、これでシーン遷移していいときに命令をする感じ
        AsyncOperation asyncOperation;


        // Startでは、open状態にして、マテリアルを取得、そして、シェーダーの強さを1にしている。
        void Start()
        {
            mode = Mode.open;//だんだん開けていくようにする。
            material = GetComponent<SpriteRenderer>().material;//トランジション用のマテリアルを取得
            material.SetFloat("_SliderParam", 1);//パラメーターを完全にトランジションしていない状態にする。
        }

        // 状態に応じて振る舞いを変えている。
        //　openならだんだん弱める
        //　closeならだんだん強め、最後には遷移を許可する。

        void FixedUpdate()
        {
            //変化の強さが1未満の場合は、変化を反映する。
            if(str < 1)
            {
                str += 1 / (float)flame;//強さを増加。
                switch (mode)
                {
                    case Mode.open:
                        material.SetFloat("_SliderParam", 1-str);//強くなるにつれ、マテリアルで透明な範囲を大きくする。
                        break;
                    case Mode.close:
                        material.SetFloat("_SliderParam", str);//強くなるにつれ、マテリアルで透明な範囲を小さくする。
                        break;
                    default:
                        break;
                }

            }
            else if(mode == Mode.close)//変化の強さが1以上かつ、閉じる場合
            {
                asyncOperation.allowSceneActivation = true;
            }
        }


        /// <summary>
        /// openでは状態をリセットするのみである。
        /// </summary>
        public void TransitionOpen()
        {
            if(mode == Mode.close)
            {
                mode = Mode.open;
                str = 0;
                material.SetFloat("_SliderParam", 1);
            }

        }

        /// <summary>
        /// cloceでは、状態を変更し、読み込み先を非同期で読み込み始める。
        /// </summary>
        /// <param name="destination"></param>
        public void TransitionClose(string destination)
        {
            if(mode == Mode.open)
            {
                mode = Mode.close;

                str = 0;
                material.SetFloat("_SliderParam", 0);
                asyncOperation = SceneManager.LoadSceneAsync(destination);
                asyncOperation.allowSceneActivation = false;
            }

        }

    }
}

