# DataParser
- CSV 혹은 Excel 로부터 ScriptableObject를 만듭니다.
- GoogleSheet로부터 Json을 만듭니다.
- Excel을 읽어드리는데에 https://github.com/ExcelDataReader/ExcelDataReader를 활용하였습니다.
- CSV, Excel 모두 공통적으로 첫째행에 변수이름, 두번째 행에 변수타입을 작성해주어야합니다.
---
# CSV to SO
- 각 데이터에 대응하는 SO를 만드는 버전(CSVtoSO_)과, 데이터가 한곳에 리스트로 들어가있는 SO를 만드는(CSVtoSO) 두가지 버전으로 이루어져있습니다.
- 커스텀 에디터를 잘 다룰줄 모를 때 만들어, SO로 데이터들을 컨트롤 합니다.
- int, float, string등 기본 자료형만 지원하며, vector, enum등은 지원하지 않습니다.
### 사용법
- CSVtoClassMaker 오브젝트에 원하는 csv파일을 할당하고 Generate 버튼을 누릅니다.
- 컴파일후 스크립트가 두개 생성됩니다. 하나는 DB ScriptableObject를 위한 스크립트이며, 하나는 csv파일을 로드하기 위한 스크립트입니다.
- 우클릭 -> create -> DataScriptableObject -> XXLoader를 생성합니다.
- 아까와 같은 csv파일을 할당하고 contextmenu에 들어가 generate를 누릅니다.
- XXDB의 SO가 생성되었으면 성공.
---
# Excel to SO
- 데이터가 한 곳에 리스트로 들어가있는 SO를 만듭니다.
- Tools 탭에 기능이 들어가 있습니다.
- int float, string 등의 기본 자료형과 enum 및 list를 지원합니다.
- vector등은 아직 지원하지 않습니다.
### 사용법
- Tools에서 SOLoader From Excel을 선택합니다.
- Excel Path에 원하는 Excel Path를 선택합니다
    - Excel 파일을 우클릭하여 CopyPath를 하면 편리합니다.
-Save Path에 저장하고자 하는 폴더를 입력합니다.
    - 원하는 폴더에서 우클릭하여 CopyPath를 하면 편리합니다.
- enum이 있을 경우, enum을 정의한 시트 이름을 입력합니다.
    - 기본으로 Enums 시트는 enum을 정의한 시트입니다.
- path 지정이 완료되면, Mame Scripts를 누릅니다.
- 윈도우로 나갔다오거나 reimport 하여 meta파일을 생성해줍니다.
- tools에 loader가 생깁니다. loader를 눌러 다시 엑셀 패스와 저장할 위치를 설정합니다.
    - excel 패스는 scripts를 만들때 엑셀 위치로 자동으로 지정되어 있습니다. 위치를 변경한 경우에만 수정해주세요.
- Make Object를 누르면 ScriptableObject가 생성됩니다.
---
# Excel to json Editor
- Excel 파일로부터 json 및 class 를 생성합니다.
### 사용법
- Tools/ExceltoJson 으로 접근할 수 있습니다.
- Excel File Folder Path에 Excel 파일이 존재하는 폴더 경로를 작성합니다.
- Json Save Path에 Json파일을 저장할 폴더 경로를 작성합니다.
- Class Save Path에 Class 파일을 저장할 폴더 경로를 작성합니다.
- Enum File Name에 Enum이 작성되어 있는 Excel 파일 이름을 작성합니다.
- All Convert를 누르면 enum을 포함한 모든 Excel 파일이 json 및 class 파일로 변환됩니다.
- create json 및 create class로 필요한 부분만 변환 할 수 있습니다.
- LoadFiles를 누르면 Excel 파일 목록을 볼 수 있으며, 원하는 파일만 선택하여 json 및 class로 convert할 수 있습니다.
- All convert를 누르면 파일 선택 여부 상관없이 모든 파일이 변환됩니다.
- 엑셀 서식
    - 1 row : Data name (ID, Name, ...)
    - 2 row : Data type (int, string, ...)
    - 3 row : Data Description (key, itme Name (limited 6 letter), ...)
    - 4 row ~ : Data
- 데이터 제한
    - 기본형 (int, float, string ...)
    - List<int>
    - Enum<EnumName>
- Enum 파일 작성법
    - 1 colnum : EnumTypeName (Rarity)
    - 2 Colum~ : Enum Value (Normal, Rare, ...)
---
# GoogleSheet to Json
- 추후 작성 예정



