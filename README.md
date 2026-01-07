# 개요
뱀파이어 서바이벌을 모티브로 유니티로 개발한 게임입니다.</br>
</br>
[구글 플레이 스토어 링크](https://play.google.com/store/apps/details?id=com.DefaultCompany.VampireSurvivalLike)


# 목표 
### 최적화
```
1. 오브젝트 풀링
  - 오디오, 맵, 몬스터 등 같은 오브젝트를 계속 쓰는 경우

2. 힙 메모리 할당 최소화
  - 힙은 비용이 불확실한 GC 발생

3. 유니티의 기능을 덜어내야함
  - Update 최소화 -> 수백 개의 개체에서 Update()를 돌지 않고 관리자 클래스를 만들어 한번의 Update()에서 반복문으로 처리
  - 콜라이더 최소화 -> 켜져 있는 것만으로도 연산비용이 크다
  - 애니메이터 제거 -> 스크립트로 제어
```
### 스토어 출시 경험
```
[경험 O] 실제 사용 피드백 반영
[경험 O] 스토어 심사 기준 이해
[경험 O] 개인정보처리방침, 이용약관 대응
[경험 X] 광고·결제·로그인 정책 이해
[경험 X] 리젝 대응 경험
[경험 O] 버전 관리 & 재심사 프로세스 이해
```

# 회고
### 개선점
```
1. 상세 기획 및 유사 게임 분석 미흡
  - 사용성 및 게임 밸런스 및 지루함 개선 필요

2. 플레이 스토어 기능 활용 미흡
  - 리더보드를 활용했더라면 로컬 파일로 기록을 남기는 것보다 더 좋고 많은 것을 할 수 있었음

3. 완성도 부족한 리소스
  - 퀄리티도 미흡하고 일관되지 않아 덕지덕지 붙인 느낌이 강함
```

# 리소스
</br>
💻 UI
</br>
https://www.kenney.nl/assets/ui-pack-pixel-adventure</br>
https://www.kenney.nl/assets/game-icons</br>
https://assetstore.unity.com/packages/tools/input-management/joystick-pack-107631</br>
</br>

🖼 Texture
</br>
https://assetstore.unity.com/packages/2d/environments/pixel-art-platformer-village-props-166114</br>
https://assetstore.unity.com/packages/2d/characters/pixel-adventure-1-155360</br>
https://assetstore.unity.com/packages/2d/characters/pixel-adventure-2-155418</br>
https://assetstore.unity.com/packages/2d/characters/pixel-art-character-package-312497</br>
외에는 Figma 자체제작 및 AI 생성</br>
</br>

✨ VFX
</br>
https://assetstore.unity.com/packages/vfx/particles/casual-rpg-vfx-239285</br>
</br>

🎵 SFX
</br>
https://freesound.org/</br>
https://assetstore.unity.com/packages/audio/sound-fx/universal-sound-fx-17256</br>
</br>

🎞 DoTween
</br>
https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676</br>
