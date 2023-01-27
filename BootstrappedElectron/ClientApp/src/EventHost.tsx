import { FC, PropsWithChildren, useEffect } from 'react';

import { ipcRenderer } from 'electron';

export const EventHost: FC<PropsWithChildren<unknown>> = ({ children }) => {
  useEffect(() => {
    const OnNewChatLogItemHandler = (event: unknown): void => {
      console.log(event);
    };

    ipcRenderer.on('OnNewChatLogItem', OnNewChatLogItemHandler);
    return () => {
      ipcRenderer.removeListener('OnNewChatLogItemHandler', OnNewChatLogItemHandler);
    };
  }, []);
  return <div>{children}</div>;
};
