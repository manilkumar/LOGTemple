

$(function () {
  'use strict'

    var carouselLinks = []
    var linksContainer = $('#links').find('a')
    var baseUrl
    // Add the demo images as links with thumbnails to the page:
    $.each(linksContainer, function (index, photo) {
     
      carouselLinks.push({
          href: "https://localhost:44305/" + $(photo).data('src')
      })
    })
    // Initialize the Gallery as image carousel:
    blueimp.Gallery(carouselLinks, {
      container: '#blueimp-image-carousel',
      carousel: true
    })
 
  // Initialize the Gallery as video carousel:
  blueimp.Gallery([
    {
      title: 'Sintel',
      href: 'https://archive.org/download/Sintel/' +
        'sintel-2048-surround.mp4',
      type: 'video/mp4',
      poster: 'https://i.imgur.com/MUSw4Zu.jpg'
    },
    {
      title: 'Big Buck Bunny',
      href: 'https://upload.wikimedia.org/wikipedia/commons/c/c0/' +
        'Big_Buck_Bunny_4K.webm',
      type: 'video/webm',
      poster: 'https://upload.wikimedia.org/wikipedia/commons/thumb/c/c0/' +
        'Big_Buck_Bunny_4K.webm/4000px--Big_Buck_Bunny_4K.webm.jpg'
    },
    {
      title: 'Elephants Dream',
      href: 'https://upload.wikimedia.org/wikipedia/commons/8/83/' +
        'Elephants_Dream_%28high_quality%29.ogv',
      type: 'video/ogg',
      poster: 'https://upload.wikimedia.org/wikipedia/commons/thumb/9/90/' +
        'Elephants_Dream_s1_proog.jpg/800px-Elephants_Dream_s1_proog.jpg'
    },
    {
      title: 'LES TWINS - An Industry Ahead',
      type: 'text/html',
      youtube: 'zi4CIXpx7Bg'
    },
    {
      title: 'KN1GHT - Last Moon',
      type: 'text/html',
      vimeo: '73686146',
      poster: 'https://secure-a.vimeocdn.com/ts/448/835/448835699_960.jpg'
    }
  ], {
    container: '#blueimp-video-carousel',
    carousel: true
  })
})
