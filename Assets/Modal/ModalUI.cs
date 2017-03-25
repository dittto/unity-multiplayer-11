// Modal/ModalUI.cs

using UnityEngine;
using UnityEngine.UI;

namespace Modal {
    public class ModalUI : MonoBehaviour {

        public GameObject simpleModal;
        public GameObject okModal;
        public GameObject cancelModal;

        public Text simpleTitle;
        public Text okModalTitle;
        public Text okButtonTitle;
        public Text cancelModalTitle;
        public Text cancelOKButtonTitle;
        public Text cancelCancelButtonTitle;

        public delegate void ButtonResponse ();

        private ButtonResponse okButtonResponse;
        private ButtonResponse cancelButtonResponse;

        public ModalUI SetTitleText (string titleText)
        {
            simpleTitle.text = titleText;
            okModalTitle.text = titleText;
            cancelModalTitle.text = titleText;

            return this;
        }

        public ModalUI SetOKButtonText (string okButtonText)
        {
            okButtonTitle.text = okButtonText;
            cancelOKButtonTitle.text = okButtonText;

            return this;
        }

        public ModalUI SetOKButtonResponse (ButtonResponse okResponse)
        {
            okButtonResponse = okResponse;

            return this;
        }

        public ModalUI SetCancelButtonText (string cancelButtonText)
        {
            cancelCancelButtonTitle.text = cancelButtonText;

            return this;
        }

        public ModalUI SetCancelButtonResponse (ButtonResponse cancelResponse)
        {
            cancelButtonResponse = cancelResponse;

            return this;
        }

        public void Show ()
        {
            if (cancelCancelButtonTitle.text != "") {
                cancelModal.SetActive(true);
            }
            else if (okButtonTitle.text != "") {
                okModal.SetActive(true);
            }
            else {
                simpleModal.SetActive(true);
            }
            gameObject.SetActive(true);
        }

        public void OKButtonClick ()
        {
            okButtonResponse();
        }

        public void CancelButtonClick ()
        {
            cancelButtonResponse();
        }
    }
}