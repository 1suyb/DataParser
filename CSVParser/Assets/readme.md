# CSV DB Loader
- CSV 파일로부터 ScriptableObject를 만듭니다.
- CSVtoSO_ 는 각 데이터마다 각각의 Scriptable Object를 만듭니다.
- CSVtoSO는 데이터들이 리스트로 들어가 있는 ScritableObject를 만듭니다
- SOFromSO는 데이터들이 리스트로 들어가 있는 ScriptableObject를 Excel로부터 만듭니다.

## CSV / Excle 파일 형식
- 첫번째 줄은 변수이름
- 두번째 줄은 변수 타입입니다.
- Vector등은 아직 지원하지 않습니다.

### CSV
- int float string등의 기본형만 지원합니다
### Excel
- Enum, List 까지 지원합니다



> CSV ver 2024 10 25 
> Excel ver 2024 11 24
> 박수연
