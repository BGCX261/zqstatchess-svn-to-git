#-------------------------------------------------
#
# Project created by QtCreator 2012-04-17T17:01:04
#
#-------------------------------------------------

QT       += core

QT       -= gui

TARGET = ZQStatChess
CONFIG   += console
CONFIG   -= app_bundle

TEMPLATE = app

INCLUDEPATH += inc

SOURCES += main.cpp \
    src/zqprimarybook.cpp \
    src/zqbookfactory.cpp \
    src/zqsecondarybook.cpp \
    src/zqtertiarybook.cpp \
    src/zqapplication.cpp \
    src/zqmovesdatabase.cpp

HEADERS += \
    inc/zqbook.h \
    inc/zqtypes.h \
    inc/zqprimarybook.h \
    inc/zqbookfactory.h \
    inc/zqsecondarybook.h \
    inc/zqtertiarybook.h \
    inc/zqapplication.h \
    inc/zqmovesdatabase.h
