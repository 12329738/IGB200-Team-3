using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JuiceBox
{
    public class PageController : MonoBehaviour
    {
        public JuiceBoxAnimation[] Pages;

        private int _selectedPage = 1;

        public void PageBtnClicked(int PageNum)
        {
            Pages[PageNum].gameObject.SetActive(true);

            if (PageNum < _selectedPage)
            {
                Pages[_selectedPage].StartSequence("Slide Out Left");
                Pages[PageNum].StartSequence("Slide In Right");
                _selectedPage = PageNum;
            }
            else if (PageNum > _selectedPage)
            {
                Pages[_selectedPage].StartSequence("Slide Out Right");
                Pages[PageNum].StartSequence("Slide In Left");
                _selectedPage = PageNum;
            }
        }
    }
}