# MOBILE APPLICATION SPECIFICATION

## Fidel Hub — React Native & Expo Mobile Client

| Document Information | Details |
| :--- | :--- |
| **Document** | Mobile Application Specification |
| **Version** | 1.0-DRAFT |
| **Stack** | React Native, Expo, Expo SQLite |

---

## 1. Target User Personas
- **Teachers**: Quick offline attendance entry, classroom continuous assessment scoring, student notes.
- **Parents**: Real-time push notifications, attendance alerts, tuition payment verification, digital report cards.
- **Students**: Class schedules, assignments, and exam results.

---

## 2. Technical Capabilities
- **Persistent Local Cache**: Expo SQLite storing student rosters, classes, and pending mutations.
- **Push Notifications**: Expo Notifications service integrated with backend Outbox worker.
- **Biometric Authentication**: Fingerprint / FaceID authentication for teacher and parent convenience.
