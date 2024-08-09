export interface Message {
    id: number
    senderId: number
    recipientId: number
    senderUsername: string
    recipientPhotoUrl: string
    senderPhotoUrl: string
    recipientUsername: string
    content: string
    dateRead?: Date
    messageSent: Date
  }
  