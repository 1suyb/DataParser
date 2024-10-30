# CSV DB Loader
- CSV 파일로부터 ScriptableObject를 만듭니다.
- CSVtoSO_ 는 각 데이터마다 각각의 Scriptable Object를 만듭니다.
- CSVtoSO는 데이터들이 리스트로 들어가 있는 ScritableObject를 만듭니다
- 

## 사용법
- CSVtoClassMaker 오브젝트에 원하는 csv파일을 할당하고 Generate 버튼을 누릅니다.
- 컴파일후 스크립트가 두개 생성됩니다. 하나는 DB ScriptableObject를 위한 스크립트이며, 하나는 csv파일을 로드하기 위한 스크립트입니다.
- 우클릭 -> create -> DataScriptableObject -> XXLoader를 생성합니다.
- 아까와 같은 csv파일을 할당하고 contextmenu에 들어가 generate를 누릅니다.
- XXDB의 SO가 생성되었으면 성공.

## CSV파일 형식
- 첫번째 줄은 변수이름
- 두번째 줄은 변수 타입입니다.
- int float string등의 기본형만 지원하며, vector등 클래스나 구조체는 지원하지 않습니다.


> 2024 10 25 
> 박수연
