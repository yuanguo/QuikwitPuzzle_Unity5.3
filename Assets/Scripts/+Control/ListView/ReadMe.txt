ListView리용방법

1. listscrollview로 만들려는 게임오브젝트에 ListView콤펀넌트를 추가한다.
2. listscrollview에 추가하려는 셀들을 listscrollviewcell콤폰넌트를 해당 요구사항에 맞게 파생한다.
3. 추가하려는 셀들에 listscrollviewcell로 부터 파생한 콤폰넌트를 추가한다.
4. listscrollviewcell.m_delegate에 listscrollview를 대입한다.

* 셀들을 추가할때 listscrollview의 insertCell메서드를 리용하여 추가한다.
* 삭제할때 removeCell함수를 리용하여 삭제한다.
이상

* 해당 셀의 애니메숀을 쓸때에는 OnSelected, OnDeselecte를 정의하여 쓴다.