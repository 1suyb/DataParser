# ScriptableObject From Excel
- Excel 파일로부터 Scritpable Object를 만듭니다!

## 사용법
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

## 엑셀파일 형식
- 첫번째 줄은 변수이름
- 두번째 줄은 변수 타입입니다.
- enum 및 list를 지원합니다.
- vector는 아직 지원하지 않습니다.

## 사용한 dll
- https://github.com/ExcelDataReader/ExcelDataReader 를 사용하였습니다.

> 2024 11 24
> 박수연
