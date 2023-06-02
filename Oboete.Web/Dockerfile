FROM node:lts

WORKDIR /usr/src/app
ARG NODE_ENV=production
ENV NODE_ENV $NODE_ENV

COPY ./package.json ./
COPY ./package-lock.json ./
RUN npm ci

COPY ./ .
RUN npm run build


EXPOSE 3000
RUN chown node:node /usr/src/app
USER node
CMD ["npm", "run", "start"]