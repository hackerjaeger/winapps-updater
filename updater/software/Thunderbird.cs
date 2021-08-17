﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Manages updates for Thunderbird.
    /// </summary>
    public class Thunderbird : AbstractSoftware
    {
        /// <summary>
        /// NLog.Logger for Thunderbird class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(Thunderbird).FullName);

        
        /// <summary>
        /// publisher of the signed binaries
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// certificate expiration date
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 20, 0, 0, 0, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Thunderbird software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public Thunderbird(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var d32 = knownChecksums32Bit();
            var d64 = knownChecksums64Bit();
            if (!d32.ContainsKey(languageCode) || !d64.ContainsKey(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = d32[languageCode];
            checksum64Bit = d64[languageCode];
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 32 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.0.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "dad54642b037a8ae90e06ac35a84bec2da667dc35f11fa5ece6a46714d3aaaced9e65ec8fe33e2f4295489cacfebc707f974dac35cf09d72a73cf53e1e2f8018" },
                { "ar", "68e613f2a3bde52351289747dba1e4c5b3c90bb6b7e047d3e7fbfcf829aa343cf31f1417aa21d6a9040010fe0760e4ea3d26898bf42fe6006c73c99ef4b78348" },
                { "ast", "4e3dfc52a02d7ff0bf7c4d832c76c2cd3c8e4cd2782374a2e475ac5a165ae31e35b8a8d8c448b93f18529e75b45151fb84c7d7695d11f6299646c15ab1a3a28f" },
                { "be", "6801b009fa949eaefafe526820a284bb2d7bb04a291d9913115e7a979c46930afa5f2e3d28dbbebf719cb72d8cc126a48cac3dd342593c4df9f93defbbd3d0bf" },
                { "bg", "6211239a07c7a7ff2eeee45fb4d0b53d5e487f2c8497909e7bb864675eca7d44a8e3f3884d0f39ff430c4e8a148acadbaf820da99f873ed98889cc2ff2bd44d8" },
                { "br", "53e5d37407ae0e5871409c88cd253d1d3038f7f3d87732d3b31ce9bae6d5a08a15501ff3ab305642eab2c5ae224de11e8bd31c0344701356ee633a4cc68dafef" },
                { "ca", "974c424256f3cfbe16733fc8f832b5538af6c51a3f6dcad3a126e792e2f4c635c0a50f9d9b00cd908f4111613c0f0c53cedb3b01706ebdc563abf0466cffeb19" },
                { "cak", "ee45cb94ae01471b7b9067da73ac5f82fd9fd3fc1b2aec1bb7e91ac25d42b40ad9ad61ee468f3d55061f0bf4c682c5e4c5a3b3971b7facfffc622b1b5a115a2e" },
                { "cs", "5ba840175bed31db62795ce1815094349325423c80c73baf05c5f09ab5e8b706152b4af04c9924c2e56a50c224c81d23390d5ab5c54543b95aff6e37afa288ce" },
                { "cy", "4a97e3547923afe9763009c4aac44c8cd6d53aa6715c0186938cb6b93ab39d71441e8a756107ae7b2a3b3eb24d61b4fea35493fd2890a7c73a1ed3c2a9d4e10d" },
                { "da", "439769c498d7092dcedceed60c6d8e0bf40b40582209a7f83bcf9afde2557f5e2e8f1b3ff0ad47d89ca2722fcae0124f94666bb4a4538c67f320f7a556da3f5a" },
                { "de", "81809d451223d8dec66a8b92f50fd4ef1dc73a44a483b14fef61d7faf92ef07716e7e9b7131616d9f6002c428b038debca632a04c3001130e4a2bd242fd45103" },
                { "dsb", "1efe86aec716874a057b4d888de77d54e2fa4d0fd124886d5a82979bad6522dce9be41e4d6d08502504712745f21bfe6ebc736c0d1199e17e5d3050ec056ec29" },
                { "el", "be4cf2976e4e71478edac953a68e41ab79da20fa987dfb2a6235e7d505f6ba246fb850ba647fbcc7ba844a73e0b686c45604918995b54b75e955a89f776d4dfd" },
                { "en-CA", "bef2146ccdecb6a03fd2173b57b91bf2e9da3cbf044e6f172015935809fd373297cbcc6abec15392f8826a65cdd736f2df0ed3fda1a1b9620b849e566824d8b3" },
                { "en-GB", "649372e731b7f06cf7a9e78c75dbe44176b23664c06dbb19e5126b261720adc510faf31c3e6075e50bca68765f794b2ddb71849461a98cabcb5be540163326bc" },
                { "en-US", "a55307bf4ff736ed3f585e4c7db4726e4ab9a5f6eb2cdbe24df54462ccbc4bcf9f684a56f8c452294eb7ef91710c5c116225c3135fa987a0f2e48883ba43450c" },
                { "es-AR", "0a69013ce0b01b4f2a36bb09072c4da6a442b1f1e44a0201d78b4b4cef20f0a3a9545c62618c3d5ab3d3a159048e2848be942ad3a1266f7e4d0185b12e0abcfb" },
                { "es-ES", "308aa19cd468974a51fd0dd4c7792bebe0c55daffce7c5e6ecf4495fabf16ac2500ac82cef954c549f26a16636198249c351697c82573e54b6699f3e730287f2" },
                { "et", "7079be936961a46111291a679aa7f513d92f2ec6f0a4f3ab6bc15b79de7aa67af0f3b9e60733496ec5de0ff38b09d2e63a469cb1be77c11f0e784df140f8d46a" },
                { "eu", "05ee5b11d7ee45e537ef3b4961ab1cb3db76796df556756b225ca09bd1f524fc2c05e96b615272d2f4d44601cf6583466930dc9a52a130744f89fa89b7fa2250" },
                { "fi", "09af490d3e49f258a135902cd637d2085c7b87ee68bfbd564062ee7e37c22f7f6e47ab2117f83ec8e96ad3647c3767b0e8df9e98cbf800026f8066ad01394717" },
                { "fr", "ca8190bc78299196e9fdbe89887184c7df05fb91abd3255ba278ce56dd021e0e684d215856863d107e1247ef4804d11c91b3b0a8b6e193f790725971f50ec758" },
                { "fy-NL", "5775d00da74267c07dd6386272cd9092140216deb2968a62c3f23d6cbf7575668b11af8f12277809dc2a450664efa43007881e0995c32dc73b7fa683396f7049" },
                { "ga-IE", "9811caa5e93df2f67e0e11d1106b2840fdb0d5b0146cfe4301f05296f9e7de419d71f849efa56dd2ac299dfbb85affedc8567da463470619915fac9951995aa6" },
                { "gd", "582a925e07fce84de586c732f1fc355615296e2f2feb33dbf3736544da493a37534e6d3f9a42e8e454f6174ee2b371207ad149822d08f57605147273d553f87a" },
                { "gl", "bf7e7410ad1f18f5ff415b56d1c059b068e3d913a123b48821cacbf206aa1afb420af72bf61555b84327b6cc97cb0592dde771660e851fe32b6d08f07c6c65ba" },
                { "he", "378fab653b2ff761b6ca7660f9f8a562066a5dbde62d9ba8de3c39331a0725b03613e3d7fca8aa780ca69838e1d08f638bdeed16b47fa9f1c94021b3d644d2a2" },
                { "hr", "648e1377e8d10cbecad79f114601a5c1590c1c063d5912dc5143b5fc0404b26e5496eeef16937dc3fd4ae08cd4908b4fdfd58880bbcd8c9946eb5c77040007c3" },
                { "hsb", "b3f440f4ce74aabeaab3cd62fab2ac0ad0735b4a2b4db178cdcfbdfa401a8cde830e839ec3ec0708bd9d6ac4dbf3ce4f6f500516b739253a028e58042edfbd9e" },
                { "hu", "634ac8b9263c6973d808f57a5d9914c38d34a53cd57833ddfd6797cb8d0ffe6d7dbc924b7a46aa0e131ce2ee36fb12d56d88c160817d69d0a1b273a213c8917e" },
                { "hy-AM", "27936c838232335c5c51cf1e29ca6d570d649bcd865f8a630f619aecb819c87edd8de80f9d384867f1a6558b0ba79b39f9ee1d94c7af5cb17d9b22d1cc342545" },
                { "id", "7388138cb46e6ed8582beba5dee6ff1b3fe2a9927879da7409db306ed56a69fd96c4f8ba0b5857d6293ebb837fbb62c4bbb154aa4d981ddc67643acab896bc08" },
                { "is", "e0e5d9fef7cf9ffb8d6726a0277eebb5f28326774d97a717b68764b6813a41af4dab9291f0fece996d62a2045182609490d2c0be54df32d33c8dce879fa9ede4" },
                { "it", "d5df58499814775c068acbf1418b1c5aad6977b81e0408c980613ad4a2b0c02becc6bd3f0a34906c21cab1a9d0af7deabedde666112819865ab6193e21e0bdd1" },
                { "ja", "0b4a30552a42ac1bc72d63c682aa122ec37b88c54054be5572b1ecde957f9bd13e4ad3d0db40c19d3986890bea5144bc892e7c23bfb05ed00639a84d4f8d2e85" },
                { "ka", "4cc976cc022ab6460ddcceb35f30aeb0d1b9913a2c49062a2e81f06c41b974e0281c09fe87fbf89469e1b2d6ea5c3c95242ff5f0f323fa7b6df452aaee49f73e" },
                { "kab", "7a511b4d8bd09952013c34fc5e2f332cc5d95936144ecc521c9907369cfa3586c8c09781d41d78906b649aa8ce47c7afbe455342446e944117532a533496f69a" },
                { "kk", "489baa19d0c32107835a47b53d605a5cbd600d54bb424ffd099b12177a85760f81514811e7d92c772fa94c232603de382cdcd729427abb7b152799eec387de1c" },
                { "ko", "7b641f8df617b282e4fa6f377d2d8996cfc45c7d4482bdd75a48b93ccf695b7dc44afca54345ebc98a3b32980c1bfaaf8cc1f7c23c9a5b47492ce82a49e2db43" },
                { "lt", "f5acdb1341d00cfc3efcbad42b5122fc8d2f10c9f25b63163580f87b417695622e3e4ae1f6f7920391ab3e6d623aacef3e4da5a3f3697838087a4b8fe022dfbe" },
                { "lv", "2dbeddaad9b772f2855ee9360e6a5e317f52094bc1f784cc293055a90c746f78f6c0d903b373e8816a6b66e5b417abb2bdfbbbc7baaaab5b969fe1baf604b857" },
                { "ms", "253a2709a392ade853ecd85c8fe6915ae36f3a3d4967798854f64b44d07b0384511cb3c08ba98338d3006d499150a2f305bfe41a79c3f27967004e15c517d74b" },
                { "nb-NO", "82ea31ed36e72f8ca30d363031c945a4c816648c2f671d9b86e1bb7275465fd278732c14dcb67abcf8e666dc023bbee02f378111d2afc2f03c5b462758220ad9" },
                { "nl", "b4ca48a43230d6c6045a5fe703148e1cf9fdbb047ce9b39c7188face8443ed16ac43f52c9bc560ad8fb908d092d9ab87da476a6ddea5ed2e5b1a7fd836627513" },
                { "nn-NO", "debad9b50898735c9e023fbfe637902f347f0597f5b32cbec3d4b583bafeec1df2fb407364f627c3884e0a14c7312394b7db17740761561a04df478f27a208ea" },
                { "pa-IN", "0b70f0b3d17a1c20ef834cde0d88f6ab711bc22daf10275f5877bfa55150448cd96faba672089a257e393b1c44c54176c1f421adc6e6ecd88b72c8f69f0e7a29" },
                { "pl", "d2bfbdc1f524b41966623f0493c4d70558252778a101ff1e72e60a37ecc87029e093c0aae365abe845c7291a766f7025fedbc3b1c385956551d8ea2de2d2f666" },
                { "pt-BR", "124fba09bab7e6dec75b2939eb17221b1f08e604974464e9636c7769d387169b9c1c508244613f9602be2b579f678c958fd77aefaed956fc62edebaadd4c293d" },
                { "pt-PT", "76696412b08bb1886a37f99528fc0716c30dd8569b99f0a964164377618e901c9ad3dafce78b9a99b02a5b25aa5e28b3a9ce3de077761ca71bb7a66849ce240e" },
                { "rm", "58e2207afee86465369ffff92b0f14cb6e6d6e9c4ee09ec917671f7d0ce048a29564f461ac5f87479ddabc8f4f857355120e478c838c4baa21afd77a285aed6e" },
                { "ro", "eeda9b0b16545c49182707941edd39fa543be7bdec80e74e088f27832d395b18f29435fe1bf8d70ca4620aa3ce49b06a6bfa211afeb12ae65c500caac39a6604" },
                { "ru", "0ebea9ca07600c2d19abfa2cda477038b6f6341a45b2b4c60eb9d1278b827322bb791fa684b40051f77df5be32884584f9457489bad393c9dad4fa6a7c3bfa79" },
                { "sk", "7d9516126edb126e49ea4028cb15aa4b4326ffb62aac3080d215b62b849e1add74cdf09bb6277237f70f62e8151a2f2aebef7cf8df85cb9a1aaf6f6d72dce82b" },
                { "sl", "95023c4ce6850f6e3557f99f0aaff8c2da7d8271e64b262f8f9d264214207a80d38f1fd191971f79716ffd5eed4d69e416fa3a191f52605102c667fa9d71ad23" },
                { "sq", "7ea952cdde7a370ca3584f1942540de32df14408243fc2e6dbe8460ab20df5f9afaf216812511d7ad45fbfc07783b3f292fe854685ea7cb997fc2815d8dbd45c" },
                { "sr", "f86cc23bffd69657c82ef60c861d0b358852a7341f3716c56cb19c0380fdafadce311c03327ac331b93ce7b559c80d489bb2230e7bc647639b1f9627819233af" },
                { "sv-SE", "9a1da06719084f35991331507fb6c189530dc691dbc5fa3fed3da1bb28226596fb993067392b243e4c6103054623b8f88e234b39da3d5185753afcf668f76efe" },
                { "th", "b774ecb430de1338ef96a9b673cecc316dd0c65ab7852410ee7bba49e4e29689b1ce7667d971060872628891e0c94557e9f08a60a985bd08d865e00a6e7e98d2" },
                { "tr", "25770df068958f71bdb331a182e1fc482b2130cf511d1867d11b6adf6a4a7a8667f0b2077ca0e03a1b21001f08802430a8bfb72fef54941c17e096e7b95ef091" },
                { "uk", "aee8edcf923c1eedae0cb1e1842faf94d159d872f024716d315d36e98423e3bd97fc7a27adf8479f7334c2ebbca541ae0f957e1269cb4ef4626f72598de38828" },
                { "uz", "a6f2156ce39809adb5a5401b79fd295e035d6ab233df99ef21d9f7447100512efc490883eb2ac95381b67ec7d6da2bfc2fc560426981e8036e9153925025f5d9" },
                { "vi", "9ba66017e6951c779d6c9a8309dd05a83ef92721b5bebf7b2cc061155c3625800959653859c0b382eb6b05caa4d3e35124f422a88e643c046ca2d7e5bbb9e3fb" },
                { "zh-CN", "521de18176e20651e1105c2ebcf9e94a88f7ac7bba30454144e1a326cbdee39a80ef42c6bd85f663b3574e845eab45163aeab1baeb6471b51151e29a5eea68fb" },
                { "zh-TW", "96101bba3c4abc73b553a9f305b683f87d4c7bee8e634ab4f2208b805ab51ef42b3452dc97583906dbb496a22781443ba7966dbe9c86aabb248e655fc719f032" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the 64 bit installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/thunderbird/releases/91.0.1/SHA512SUMS
            return new Dictionary<string, string>(65)
            {
                { "af", "50a5b11bf3407a5b32d21b666cd5e82d61cf7f6703c21291275905b8795fa1870ae30a9cd48dd018483670da8de6f7aee8cc3410517eeee8e0f8d3297f9fd279" },
                { "ar", "ea41593eec1aed67932312254e266bcaacc96a9de44b4903e4cb37b4acee569db1cb9e66923eb7d2b7b0d9f87fd83ae242affa58cc2e6c150e71a2284fd76abe" },
                { "ast", "fc4285def03c7ce37e1c4b11b3c7dcd77edb9f9d008c2d7833e3ef44bb60fd1ec4130071d95a8afdb0a39853c31164b1c70e97b11beebc7e8dcd2fd0a395cd44" },
                { "be", "f1895cf1c2c1635dc978d611d54c6f337a0634739fd686e8020f3cf013a76316dab1e1248e01cac9707648e7a1cc7c9ad5a73f9c6733277ae12c12fc2ad95b2e" },
                { "bg", "d9be436bbd003d927dd7901b6145f3dcf6cdbf036ddd93755474ed4be619e9774d3f223673d7e29057ef89e4409de44ba747217458b2bb36eea8ece0a79742cc" },
                { "br", "49f65e1901b782521e644cb66b82b4349b37cc91a3f390b0e87ef7d561df4401de9a6a5b7f8c8ade474255770b4d90fe91b6814474d872bc39731fae0f1fea57" },
                { "ca", "72dc4f67f11b8053edc8f04cfb0e1a411c88664663eadbc369a9988d2df3ed311c2b71fb5dd75f783fc15ea8ea8883211f4603114dbc7da6b2a5fd42dce1a3f9" },
                { "cak", "56679d3f5917bb6ff1ebd8d32f991ffbbe5105de29986e973b962daa52bd63f633758738f6643ebf91407e1de0499c8e649cd0810184ae12b047c66f842b95b4" },
                { "cs", "fdd627bc715ac50910146918feb39826572b13cbc5a29f0ccb69763053e37cdbe56a4c85dcb667cd7026979185becc6153bc9bd364761c53d771e1bf1be44d57" },
                { "cy", "5b0b508fd4140cf2db4cf6ac663ca9aead3d7c6c782d27668a65218c301339b69a4f87692a9c80730c3cd3365dccfae415722b4a8dbaacb1873ff739ea16c285" },
                { "da", "cfe296705fda5b83217f3aacd10ef33665d7a0852a6cade1f20027ddae7700045dc59b0e42c9f59ecd624539752d889d5a02eb49a7cce2088659a4fdf6fd527e" },
                { "de", "b14ea50fe7533d35114695b495294e2a5c6a6097bf2f37cc2a3955238369f8ce66008fb3e5519d9d8c6daa21b6500faca2c3cc1caac9fafdc9fc90d9bacdd9e0" },
                { "dsb", "d7d269bf9166a23df22aa6380009a1c0c1260e12a07792f86933474f9d22224626d1cff1057603d17b03f20e4fc5fd9b670fa5ad8fe30931bbd57055bcfe9d9e" },
                { "el", "5df67c9b181d88633d4764d7872379be9919aaa20f31454acc21d0713ab4eadefbba726f3dbb557e4f824b646b7a6f9578bf043736f1709b15b5e42147386904" },
                { "en-CA", "65dd4f22cca68bce906e04d94598e20977e1e6659b8e4ba4d7bafb444ab1521384c16559202e1669b26b59d4659be42232b7f12f41a1031a7552f113e03d0807" },
                { "en-GB", "24ed3e864e68ee134ee1ad6fb4344e4a1dbb41f7845502e4c0eed72e249e9a51cbc117d06a9857e6333002404afbd87bbda6ee061f7e2d9c0c86f46381a8da69" },
                { "en-US", "ae6e48d6658bcb7fe4e9c458bef0c92304e99745d811decf0af8a5d6a78bba23c8bce0988e3e424abd451c5782d9d61defc56b020c0c2ec08d6cb0d71b911e7d" },
                { "es-AR", "8b2c3adbd6db1560b74e3fdcd8b234cc89d8190bd080ac39c19853d328c650af442ff323f122a500d6512ec35b2d208234b2935319672dce725567cb0d706234" },
                { "es-ES", "a0fdbe1e42de14c14ecef80b163595779b59bdb96fca967dc34b8c47accbfab0d0c5f75472db3edacd28e7668e97bd413d3b3b836245c0b79422c27bb4add53a" },
                { "et", "7aa08e7b07e25240aaaf31f3ddedc40fd156aaf9c2cbe132bba271c2e9ee0655467ebd6ca4daad54449d07a452fbf8fe6d3074e758c724462a11b96fcf7037aa" },
                { "eu", "d33b040720ddf2f0f0e3bec91279a01fe59b1b641262b1d35e2b103c1ff41b13cb6dc277c00d2edbee87c50e810b4e3a5e1d2c57ec2e0ac9cba3600686044bfd" },
                { "fi", "350ca36802d3db639e376647f394e0366e9de7ac6bcf5e480831eeeb3b8bd1ed64bf58968a792eedb35f4e9ad722a8505c7e5929d0ab23b1747597b6ec4cd18c" },
                { "fr", "5625334bbe7fb4abbdc64ee9a7b095173b134411149668bd4ff3494038e0f680a74f674afa82f1b8d16901189bb4e24103ac686275dd9808145ee36e6aab71f3" },
                { "fy-NL", "4397092d9e10a055263e3ac37583e786dc9972342b153241e8fe1600b8b0f76c178fc14a0cb7d8f33b06658ced240ab094449de27e3fbc852a6d93c27df01398" },
                { "ga-IE", "876ca118b8192000819dce9659dd2fc8157afc2230efb076b41c34d36e8e5c43f79ff266846a33dd910975040c6025048100951befc64aad2ed1b48b376c11ba" },
                { "gd", "f379e152fe31335316ea5beedf491a4c56de6620c0f6b51c6985253517c523475405a4b5436487db9f3ae978d0f70aa3aabb86db8e1485c7e9b97ff184bc057a" },
                { "gl", "07b5741c7877aae7325c9643e3c1a8386409985ec0faafbd42d51e53ef4da7f1fcfae912f2bc645c94c2373aabf3047c841ca459a72f13ecb8680d9e56e9d422" },
                { "he", "0c33dd52e5fa38024af822820365dd5c9f0adec31e665f5504495fa1f930dc6469e0484f206a231da050f22a1947a3bb48c052a61e7fedbfc4ed784dc3a077be" },
                { "hr", "59622e29d5c345f253f22c8b714b8ba0fd7a3ad8ed5e5f5eb90858e4fc0eb495d16f53bfbca331d827d229e305cbe2465417b9f4abf4f640f65d658f2d005b3b" },
                { "hsb", "e555a98d8e56b16aeda6b041fdca59743cae940308dc8e533f61ce4a03731c615f2f42d98c081ec068be50587bcd6517a3a4ff6b71ce36064d2a66cda35ea157" },
                { "hu", "dc2e116490aa79f177ce6d7007ab5d8435546f542e89649edc7f2a992132ce007cfc2793b4cd43a87ea6c118e40de40b8c4420efebff1a4e308d03e982914b18" },
                { "hy-AM", "b609ff8768bafcfd96d4fc4b9519a607860ab64f34962b2ff0df948da79072a0c3fd04eee50ccb8629f70f7b22156b277d8bc1b9e4f5f98b9aab6aef0bfe28c5" },
                { "id", "1bf5b0a6aeea01f2732d48a2e0b36008f1dabc5bb44225807b9d45313219f93a6e562dc1d15df9b025328146b45e37d78d9cc3f256e15e1901dc215180cb3147" },
                { "is", "53bc3fec7ebbc913336191ba075188b0a12f81f8ff2c366a31679d5f6c4fd9f59c0b38b420b0e8996497d06c12ef4dc53ce74a600e3f653da4380f3cca1f257a" },
                { "it", "5949fa8b40a4beb0922bdf5ba3590ebadb048a6296dd87bb71918f7dff4d381d11a38ff9e515e3854b09bd5ddd4cdfc758026c323074b90d60021682ee3b27b2" },
                { "ja", "b473e8d8a21e9f7c500883331c248848ada827e5764d5ea926a2021c66d02d933856fd6ff98efb8c7f9faa235d9417756d87e7eb1f39e20c6d5d810a06258371" },
                { "ka", "22bbbf6d855d7ffad26343c073d27df40d82843964e70d332a901c1433af5c5ffe4cd496e3103c5c29277b5adf90c4ca41c4d4490dd797c81868748aa27bf483" },
                { "kab", "da04a9b20a514beb7515d15883f6e19d98e205312e0b7a3223d722e417cefaf07c7e2aa90de5d28172df69e72820cff535a50bf116ba628b5dfe4d2e8f512385" },
                { "kk", "5e3aa4c39f059c6c45265c2594a80e33458c0a22234ae06bcb4542548d626f68b1cf9a91c0b72c6e67605cbe4aa90820ddffb65029538c8e265f9b6372596afc" },
                { "ko", "b5636424c14dc101ac45014b7bcde35b0fd7777c6e728271d69354ab282d3fd6863189fe9811ae8a87f7362a55f1b9569772296625ba01be10c2c411ba74e1ed" },
                { "lt", "a6659f1a5edec3afb7aca3e118fe4416fc6c15205cdc67f836ab8d3d6400233f426a040369cb0b67646ab4ba5b11d170649d2140edd02912c921436083ed8d82" },
                { "lv", "5fb0dc9df6df4e952009bb855dbd52911c50782649bcd78fef8ee989e412e2b5edfd254579751b8fb96b2e201d3b2d56e36836448a4ea7f0458b5d0b483ff05c" },
                { "ms", "aa36a6c59c18e7924eeda55f7d27014531becf951b8df8d91f8d237a7185e136f9584281e499d004510863e326edfd8fe2cd75eb684b5f3d864abfc83272f6d1" },
                { "nb-NO", "9bf0d7234e6e43a73bbc7f989c0ed6cf5ceedafc249e3cc8bbe5e18502aa91b06c82ca7b8ad812329313577f0135ad56e9d40316ea66b21c89d9bea08a872e62" },
                { "nl", "ea50600de9bd9119c9387f9c55b1353493cee83e0c6d25188650ddfc56a6d69aed3f6f052d26cd189624c883ec4ef8602e6fdcb26b1edd1a56287ad58d2a00d4" },
                { "nn-NO", "758a2ca250e2799704b2ee4c12f19f037092f0d5f1c87eb2eaf153fe691a31c270ed57fc9b741b365579a2074de00acae9913d93daa060781562b529c01ca61e" },
                { "pa-IN", "ee5d99d009ceadd7d37832b1fdcd32c97a5f3a06e907e0c33dd580ab32dd20c518a10c0743b50052c3e07c48cb573f1cd77a4d756bbf4f200842f7affec9b498" },
                { "pl", "904d417217098f0b9c5089f328928f4fb6f93b3efb3f0880449dd1c492bbd3502d5a5c27612e6b3b0fbee62a6cbfad6a5d2b1748210e4d2f03a74b68179b0a55" },
                { "pt-BR", "0c045e4b7d0f9a3d83c951f401c6046a6fcafdcfc21692fba0b92b5d5d864631f4e1c7d4511a654118d47473774da40e4b0ef3ea62c0ec32dc20996d49522364" },
                { "pt-PT", "3f225995fb4387852c6dadfdd253c06550d8fc65576452950cdcb2235a86eca088b609431663612e7e0a1f5cdc1d3fe1fdef8e54e9227c13a6e36e2bdea99bf3" },
                { "rm", "b952c9e98f4d0a799214dd366dd56b73cf5f14c4ecd07bddaadb38ec4b71d52bf46d9f29e2c1f547e581e6d5392120b1887ae7cf62993951431f666d07698a24" },
                { "ro", "3d05fb0d0a23d27fce72f37129c0efd9b2e488be967ac496663ebd02019f2f72c4587ed750bd05d3849cebb04cccd62136ef6f9d411b6e0dd1861cd68f389b21" },
                { "ru", "9f58fe877974570e8b921e9dbe7c4afa427584a5d082009f62704678bc9234cef93a752dc3cfe7eb72ad18934fb189d7f767fa2b9fc65c559da828c81ae09761" },
                { "sk", "ab6d7f4aa086ea9a971455c07374f74d17018031bf7dcc83162ed4dbcc7f5d0ee2cea4c4b147bdf2089eb8ba9905a911578421d1d192dd66b492363174267a52" },
                { "sl", "782bf3d59e4fd5c2a029a7099cd85fa20caf3df1d1ef70b222d6483bf88f5eaf48d6a6eafc3d86b4e4a09e29527530878cc3566c23faf3bcb88b352a29918a9b" },
                { "sq", "cec99831a1e8fbb06bc8b0787bbca7d195cf2f2b2474e0bf1b00f1c73d5df43bae5b9a1308a5703dd61ce9da82018cabc70f891e4231eb98695c54788f8c544b" },
                { "sr", "32e5949452473deed52d703443fdf4afa93fa5c0c52c48a0afdabbcd7c61f30d06d77327ff09ceb0463b93a0dc6e0429b2ce537d09cd80579bcff0f01b12da61" },
                { "sv-SE", "a125f99ea0f3f5a2bc45c54976a0faa9dca33d5b3e4a074a83c8dd09963d98bc3058961369660f3204bc5efb861616e192b8c7e6c52724f512d1bcee93cc07c3" },
                { "th", "a8a5db18187486083bb1ac5138a900b1fa0e1f340222d6b4f93a55e03d90db13f3bbff3166572987933861cb6b6014074377b0751beff12ffd1e55d2b79ab6cb" },
                { "tr", "e7fd2718a0f29f9bbd5ea913bdc3c2f38434d7b57b212fe21ca3621fcef8a29036058018204937bb6413fed018c345a1f4bc7b79a0c0d50dee55a4b412395fa3" },
                { "uk", "aefb3e482124087bcb09acb52dfbf13358aa809294dc2ab4af7166b9efbbc8b52e7a119cfa371523b833b8ade88140873dc2e4a1abe153645cc2db8669bc2d06" },
                { "uz", "50c05c169d7d12c1104c3b37b6e82517ddf9f27721393bf805771d664e98aa4da57a3bf59ac68bd527685012a095300889981847d019ca78dfcde3cd97f78c88" },
                { "vi", "f2dfd61446ea61ec2b10be65449f3d5aa6b35bb54d83ccc65cf287e367d97672f49452e40e1eecf460a9b5522cd4bc1e97e1c370f7b14ec1d0b4e55c6b5ecbff" },
                { "zh-CN", "cc4b00b9a30ca7af76a8b93a2db37b41b56a655da251bb84771c3f6cc376dc86ff9d4e1c159edd6a26a51feac55609a87ebe19e372dae5ffb14ab13b82d727c1" },
                { "zh-TW", "d8154c5d0d472f86edf8b63102b3f20527b60de11c36b21310a19e1ce244ccc553f6de707bf12ff40eb5fea9a12a457fbd7223798f0d8e63640366a87b292c45" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            var d = knownChecksums32Bit();
            return d.Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            const string version = "91.0.1";
            return new AvailableSoftware("Mozilla Thunderbird (" + languageCode + ")",
                version,
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Thunderbird ([0-9]+\\.[0-9]+(\\.[0-9]+)? )?\\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win32/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/thunderbird/releases/" + version + "/win64/" + languageCode + "/Thunderbird%20Setup%20" + version + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma"));
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "thunderbird-" + languageCode.ToLower(), "thunderbird" };
        }


        /// <summary>
        /// Tries to find the newest version number of Thunderbird.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=thunderbird-latest&os=win&lang=" + languageCode;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            request.AllowAutoRedirect = false;
            request.Timeout = 30000; // 30_000 ms / 30 seconds
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.Found)
                    return null;
                string newLocation = response.Headers[HttpResponseHeader.Location];
                request = null;
                response = null;
                Regex reVersion = new Regex("[0-9]+\\.[0-9]+(\\.[0-9]+)?");
                Match matchVersion = reVersion.Match(newLocation);
                if (!matchVersion.Success)
                    return null;
                string currentVersion = matchVersion.Value;
                
                return currentVersion;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Thunderbird version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksum of the newer version.
        /// </summary>
        /// <returns>Returns a string containing the checksum, if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/thunderbird/releases/78.7.1/SHA512SUMS
             * Common lines look like
             * "69d11924...7eff  win32/en-GB/Thunderbird Setup 45.7.1.exe"
             * for the 32 bit installer, and like
             * "1428e70c...fb3c  win64/en-GB/Thunderbird Setup 78.7.1.exe"
             * for the 64 bit installer.
             */

            string url = "https://ftp.mozilla.org/pub/thunderbird/releases/" + newerVersion + "/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Thunderbird: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Thunderbird Setup " + Regex.Escape(newerVersion) + "\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksums are the first 128 characters of each match.
            return new string[2] {
                matchChecksum32Bit.Value.Substring(0, 128),
                matchChecksum64Bit.Value.Substring(0, 128)
            };
        }


        /// <summary>
        /// Indicates whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Info("Searching for newer version of Thunderbird (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if (null == newerChecksums || newerChecksums.Length != 2
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>(1)
            {
                "thunderbird"
            };
        }


        /// <summary>
        /// Determines whether or not a separate process must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns true, if a separate process returned by
        /// preUpdateProcess() needs to run in preparation of the update.
        /// Returns false, if not. Calling preUpdateProcess() may throw an
        /// exception in the later case.</returns>
        public override bool needsPreUpdateProcess(DetectedSoftware detected)
        {
            return true;
        }


        /// <summary>
        /// Returns a process that must be run before the update.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a Process ready to start that should be run before
        /// the update. May return null or may throw, if needsPreUpdateProcess()
        /// returned false.</returns>
        public override List<Process> preUpdateProcess(DetectedSoftware detected)
        {
            if (string.IsNullOrWhiteSpace(detected.installPath))
                return null;
            var processes = new List<Process>();
            // Uninstall previous version to avoid having two Thunderbird entries in control panel.
            var proc = new Process();
            proc.StartInfo.FileName = Path.Combine(detected.installPath, "uninstall", "helper.exe");
            proc.StartInfo.Arguments = "/SILENT";
            processes.Add(proc);
            return processes;
        }


        /// <summary>
        /// language code for the Thunderbird version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;

    } // class
} // namespace
