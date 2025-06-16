## 📌 프로젝트 정보

**프로젝트 명 :** <**완강기 VR 안전교육** : 완강기 사용법을 배우고자 하는 현대인을 위한 몰입형 VR 교육 콘텐츠>

**키워드 :** 안전 장비 교육, 유니티 VR, Hand Tracking, 몰입형 경험

**팀 정보 :** 217 더보기(04) – [@yoongang02](https://github.com/yoongang02), [@JJeaea](https://github.com/JJeaea), [@nyj9800](https://github.com/nyj9800)
<br><br>

## 📌 프로젝트 요약

**프로젝트 설명 :** 

본 프로젝트에서는 실제 재난 상황에서의 안전 장비 사용률 저조 문제를 해결하고자, **VR**과 **Hand Tracking 기술**을 접목한 몰입형 콘텐츠인 **<완강기 VR 안전교육>**을 기획 및 개발하였습니다.

이를 통해 기존의 일방향적이고 컨트롤러 중심의 교육을 제공했던 VR 안전 교육 콘텐츠의 한계를 극복하여, 학습자에게 더욱 유의미한 학습 환경과 경험을 제공하고자 하였습니다.

특히 Hand Tracking 기술을 도입하여 학습자가 실제 손으로 안전 장비를 조작하는 듯한 경험을 할 수 있습니다.

![Image](https://github.com/user-attachments/assets/aa8b12ab-660b-4fc3-972e-0632860ebc15)

<완강기 VR 안전교육>은 크게 **‘튜토리얼 > 완강기 학습 > 퀴즈’** 세 단계에 걸쳐 진행됩니다.

먼저 콘텐츠의 기본 조작법을 익히는 튜토리얼을 진행한 뒤, 완강기 학습 단계를 수행합니다.

모든 학습이 종료되면, 배운 내용을 다시 확인하는 퀴즈로 마무리됩니다.
<br><br>


**주요 기능 :**

![Image](https://github.com/user-attachments/assets/2d4ae950-e648-4577-998d-58d0460a9830)

1. **학습단계 관리**
    
    ‘학습 단계 관리자’에서 모든 시스템을 제어합니다. 목표 행동 완료 시, 자동으로 다음 단계가 진행됩니다.
    
2. **완강기 조작 및 행동 범위 게이지화**
    
    Hand Tracking 기술을 통해 실제 손을 사용하여 현실과 유사하게 완강기 설치 및 조작이 가능합니다.
    
    물체 조작 범위를 게이지로 시각화하여 편의성을 높였습니다.
    
3. **하강 시뮬레이션**
    
    현실에서 체험하기 어려운 하강 과정을 직접 경험해볼 수 있습니다.
    
    IOBT 기술을 통해 학습자의 상체 자세를 인식하고 교정을 해줍니다.
<br>    

**데모 영상 :** https://youtu.be/IbRqjO0MGuU

**사용 SDK :** Meta XR Interaction SDK, OpenXR Skeleton
<br><br>

## 📌 설치 및 실행 방법

### 🛠 Unity 설치 방법

| 단계 | 내용 |
| --- | --- |
| 1. Unity Hub 설치 | - Unity 계정 생성: [unity.com/kr](https://unity.com/kr) <br> - Unity Hub 다운로드: [unity.com/kr/download](https://unity.com/kr/download) <br> - Unity Hub 로그인 및 Personal License 생성 |
| 2. Unity Editor 설치 | - Unity Hub > Installs > Install Editor 클릭 <br> - **2022.3.50f1 LTS** 버전 설치 <br> - 목록에 보이지 않으면: Archive 선택 → [Download Archive](https://unity.com/releases/editor/archive)에서 설치 <br> - 설치 옵션에서 **Android Build Support** 체크 후 설치 진행 |


### 🥽 VR 기기 세팅 (Meta Quest2)

| 단계 | 내용 |
| --- | --- |
| 1. Oculus 개발자 등록 | - [developer.oculus.com](https://developer.oculus.com/) 가입 및 로그인 <br> - **My Apps** 클릭 → 신용카드 등록 및 전화번호 인증 <br> - 단체 생성: **Create New Organization** → 이름 입력 후 동의 및 제출 |
| 2. Quest2 세팅 | - 전원 켜기 및 계정 로그인 |
| 3. Quest2 개발자 모드 설정 | - **Meta Horizon 앱 설치 (모바일)** <br> - 앱 실행 → [메뉴] > [기기] > [자신의 기기] > [헤드셋 설정] > [개발자 모드] → 활성화 |
| 4. Meta Quest 링크 앱 연결 | - [링크 앱 다운로드](https://www.meta.com/ko-kr/help/quest/1517439565442928/) 후 로그인 <br> - 기기와 연결 <br> - Quest2 착용 → 기기 내 빠른설정에서 연결 진행 |


### 🎮 완강기 VR 안전교육 콘텐츠 실행

| 단계 | 내용 |
| --- | --- |
| 1. GitHub 프로젝트 클론 | [VR-Descender-Security](https://github.com/217-etc/VR-Descender-Security) 저장소를 Clone |
| 2. Unity Hub에서 프로젝트 열기 | Unity Hub > Projects 탭 > **Add project from disk** > Clone 받은 폴더 선택 |
| 3. 프로젝트 실행 | Projects 탭에서 **VR-Security** 클릭하여 실행 |
| 4. 관리자 모드 경고창 | "Unity is running as administrator" 경고창이 뜨면 → **I wish to continue at my own risk** 클릭 |
| 5. VR 기기 연결 및 실행 | VR 기기 착용 후 Unity 실행 상태 확인 |
| 6. 씬 실행 | `Project > Assets > 01.Scene > Start` 씬 클릭 → 상단 **재생 버튼** 클릭 (VR 기기 착용 상태로) |

<br><br>

## 📌 폴더 구조 및 커밋 메세지 컨벤션

### 📁 폴더 구조

```plaintext
📦 Assets
├── 00.Test        # 테스트용 임시 씬 및 스크립트
├── 01.Scene       # 메인 및 서브 씬 파일들 (.unity)
├── 02.Prefabs     # 프리팹 오브젝트 저장 폴더
├── 03.Scripts     # C# 스크립트 파일 모음
├── 04.Materials   # 머티리얼 (재질) 파일 저장 폴더
├── 05.FBX         # FBX 3D 모델 파일 모음
├── 06.Shader      # 커스텀 셰이더 파일
├── 07.Textures    # 텍스처 이미지 파일 (png, jpg 등)
├── 08.Animations  # 애니메이션 클립 및 컨트롤러
├── 09.Audio       # 효과음 및 배경음악 파일
└── 11.Sprites     # 2D UI 및 스프라이트 이미지
```

### 📝 커밋 메세지 컨벤션

- **Title**: `Type: 내용`
    
    → 간결하고 명확한 요약
    
- **Body**:
    
    → 세부 변경사항을 설명하는 문장들 (선택)
    
- **Footer**:
    
    → 관련 이슈 종료 시: `close #이슈번호`

| **Type** | **내용** |
| --- | --- |
| Feat | 새로운 기능 추가 또는 기능 업데이트 |
| Fix | 버그 또는 에러 수정 |
| Style | 코드 포맷팅, 코드 오타, 함수명 수정 등 스타일 수정 |
| Refactor | 코드 리팩토링(기능 변화 없이 코드만 개선) |
| File | 파일 이동 또는 제거, 파일명 변경 |
| Design | 디자인, 문장 수정 |
| Comment | 주석 수정 및 삭제 |
| Chore | 빌드 수정, 패키지 추가, 환경변수 설정 |
| Docs | 문서 수정, 블로그 포스트 추가 |
| Hotfix | 핫픽스 수정 |
