include_directories ( ${FREEIMAGE_INCLUDE_DIRS} )
link_directories ( ${FREEIMAGE_LIBRARY_DIRS} )


set(TEST_SOURCES
MainTestSuite.cpp 
testHeaderOnly.cpp 
testChannels.cpp 
testImageType.cpp 
testMemIO.cpp 
testMPage.cpp 
testMPageMemory.cpp 
testMPageStream.cpp 
testPlugins.cpp 
testThumbnail.cpp 
testTools.cpp
testWrappedBuffer.cpp 
TestSuite.h 
)

IF(ENABLE_JPEG)
  set(TEST_SOURCES ${TEST_SOURCES} testJPEG.cpp)
ENDIF()

ADD_DEFINITIONS(${FREEIMAGE_BUILD_FLAGS})
add_executable(Test ${TEST_SOURCES} )
target_link_libraries( Test ${FREEIMAGE_LIBRARIES} )

add_test(NAME Test COMMAND Test WORKING_DIRECTORY ${CMAKE_SOURCE_DIR}/TestAPI)
