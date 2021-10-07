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
using System.Net;
using System.Text.RegularExpressions;
using updater.data;

namespace updater.software
{
    /// <summary>
    /// Firefox Extended Support Release
    /// </summary>
    public class FirefoxESR : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxESR class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxESR).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new DateTime(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox ESR software,
        /// e.g. "de" for German, "en-GB" for British English, "fr" for French, etc.</param
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxESR(string langCode, bool autoGetNewer)
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
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/78.15.0esr/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "8a79ac01c7508cf73d5e324b97f0e034449b7df7767dc7402739cde385ceb5f741b2795ab1665f49b68d901c8ff0cf7c3aec769f579cc030f599f2555e3e6ff5" },
                { "af", "dfde38b795a5af672a6eaeee56f983eb4bb373fed0508a92c738b3eaf7ab8c1a928359773b4d1e3b9d52a6974c289a0acafa33f411c615ca57af12010231dc39" },
                { "an", "a3890a93b9694d4310ea5ef8a7ee8618ab48b451267d398888d1c3e251025c43baa421a02f460f202cdce367851b8e4800e44ea8bac8ba0eae1078f1428dca26" },
                { "ar", "1d33449115df19bca0b24467dcfe6cb92f13d255db52dbfba21ee93e1ba5f39ece345b7cae31cd67301749a9455d3d854945688293b869ff79b53d323ea03401" },
                { "ast", "beb9173a86ad6457e49851ed23c0e18b4b2550fa1212a19109daf39c3314c4e65a74fe348fe8a6ce2eb62aecc479af1f0c3ff532b95659c86e2fd9e7f0f08c63" },
                { "az", "c5884c5af6de6cf1bbe675a2c92c2558d7e5d6c50bedd5e1d2aa659c98be495db1fb1d4469b248cce5924c8e6c5fd83d60af59ddd62d51ca407053d57c2c994b" },
                { "be", "17de5ef02f58fe3b7f05fd8266ba145e767ee52045e16c4e0ad263344ba91c1304d2ab8c8d330ce10bd4396121b62b6adc6ef8f606ddf7345879fa7255f91154" },
                { "bg", "46716a0af33ab56af1f53add1c41ae0bfb0eb213e2e90bb4067c50b25cc1e4f0e23f7754dba90b452d8fd7da5fdc12ab8c3f95ee9f7235a6197dfe7e749aeb05" },
                { "bn", "5b9cec0cba73fdeecf06a34cc07d5e1f7b70b7d6e9129a18af0352658030c8bc0ec75f8c0eb6ee945458f15f9f4d0dbae5ceec53a7a7ac8a51244774941272f0" },
                { "br", "a039cd3bb1b6fc1c817aa7abcfa2a11e3f44c80a2407e1b79844ed1ef64a0d48a711a907659bf68a09cc9b54d050d4b26f84347b80a2cc0920caf9c0ee458521" },
                { "bs", "651fab52a79a9ad2b2eb1e6df1a4463bb17d46668521768f40ea50d3627ae7032c192fb0ec6a956006a9395b5f2eb749f75db880b3447e118a9df1000440864d" },
                { "ca", "931fbc4cc1c2ac3f21e264b04d1fa7d81f631ad4cda54216de341aa228018b22e7dd74b030332b50311ba23bd303f5ef35cc408d013d937df850647079747073" },
                { "cak", "11ddc31d625757ea5be5d65b1f9b2826be60bb83b2028828c16f3f75191ff353ceb385398d3e39f21f6557623c8f45c55f089d90853868647af330830b5b6884" },
                { "cs", "76d428e7dc9026df3af3094be4c3c5d971275e4f255d5d23ca034393084ce6ed0880909ae77c90afec9aa5fc3b2087c36f37551783d7bd83287e79a341034963" },
                { "cy", "c311cb71e09b0543cf9a67773ade04ec7540d063bbebbac7f5d46f9c628f984acb1ab641fb8a4432bfd155d67558155f7685c7b2fe3a608f8aaff2647ffe3311" },
                { "da", "196057745812282797abecc354eff7147990afbd5069abd4a5faf2e4dd7ebbe397dcd8ef41fd5a9bbc665d4b0f2c3105b8a02957543d1cd7683ee2383331ac8a" },
                { "de", "3c3796c56c9ee9d984c9fb0a325e97688c612bd9b251592b562d9e4bae06f94d9f59c6e113b7c3343d088bb7585bfe496e91e45155213a1dfeb44729abb43f67" },
                { "dsb", "139cfa7d05e32849c0afc90d1f8b26a2de5c4142408f18c4c3af8a10a77ae5e1154ea4b70ff902b54d7ea14981e82d9e70f91412271724f86884910796bf14c4" },
                { "el", "da0ad576b462a85671fa5109f709b7da39257fe072f53d5df0a671a6f43e6328a53968c38871ddb6ac7e56d176bf0a769a9d260a8e7445d9ec18051478f6d798" },
                { "en-CA", "2ce24a8f09f38b6c953b3f9b8f6de61c109ffe4242ce5451afd9ef2dab3394fb03da12f1ab6aede032d891aef8a414d3e79cb8733d33e6cf04a6e774fdbf5435" },
                { "en-GB", "543f52005d954d8a8a76cb62c4eaffcd1041b6f643e8e3b28168e59a6937e9ff8d7a3b9391dc758cd8c3da429323ffae3a47f5f6b8b30bc17b2a27c84838f0f3" },
                { "en-US", "1b9e45ba9acc4a3e001caf55ba2c2d95590e1181967bc270e7ec55dca97bc40fd06644e190cb8d83dc52bbcd180d77fd3234b8fca4e887ef6faa5a204026482f" },
                { "eo", "05e026d2ae0bfa77c8afec0ec9625757ffff489f888b9cc0f58bd699c6cf6db225e05c8d0ff83e8ceff2f721e265637f5527859d055ec14addf56732f6cec638" },
                { "es-AR", "c6e04f3b6bdc11eda83948c90125593a71366b77379865e58dd3cd3ed9aeb94ac47dd0a64cb8685599eeae2a4039e279f4d3d59497327d9c870af7acc25f03da" },
                { "es-CL", "31c3de8f01bd3828afb59bae0bbc9e44f311b8548bfa907955b27740d325ca3f9997579c40802a31155da19f0f19fc7f1cb414335af4ef9779f6e47e2fa28a49" },
                { "es-ES", "8105112eb091e8245285d6846ff6caf22a22c0d1af1c04b72b9e26a743ad818588a155978c01c06b7a3a9d9c843e999e0f6efb3efa6f3d8f3549dfb548eeb734" },
                { "es-MX", "e0d6de11cb03eb3f1f0d2e105e88c1d195fcb87f7dd6789cebca70187a1b95bf9003ddfc55b76c2f3526bceacf6cf87254f87ba002d75e91c80eb81b8bd0b7aa" },
                { "et", "3cbc22c2498ba19a9985923630271927b17eaabb168af2b6f519ae45e6e6dfbbcbf0284b245ca7455f751e3885d0a03d8cc304572df093e2c28e712339180aec" },
                { "eu", "7f8e8c13805f80d0e97b69405967e58b5209014e48d2aa6291ba75d5b0697a4abe93fdd346bca2d0712304f2d47d43f7c7f9ecc6e153e08020c0854037fe9ac5" },
                { "fa", "5ca229cea5e047bae2c374f1dcf7301956c27d5b71a274325995e26af0fefed345dbcf1fe25f49b67db0869d89e77cd8e96562cbdb8567f23ae58a88742b2764" },
                { "ff", "b69b3871542cb9b1787de864d992557bac7a3f1510aa8b17dc2207a91b7915ddbc1e5b3434c2206cbd9942d0a67dc3aa2f8734e5b68a14eea0b3dff1a842bcb5" },
                { "fi", "fcf0bd91bab258ae86e0a1d42cb23c5fd3836207bc79ca45e829256fb9309b58c8b904a3675e85c393a56e01d43b844e4fb1ada40ee43295d92e8a7689ca5606" },
                { "fr", "793980275223796f8d7ae17ee57d91ac9d70bd5d52e570adb6fc53ef651a376ba127830f23553a820e7dfff214da301a266804e3f312dedf164661cc735edbd9" },
                { "fy-NL", "0a9963025d3c8661cf92cf0586213ccb761ebb8c54b56b508c7f5d2cbecb72488a2c52fb2ad175cb7700f5f19a9d08e6835da2d748b6a6c95f4c54d1420ae10d" },
                { "ga-IE", "aed6704649e1bcff87751025a30e2b223784c06030930ee776fe9ad1f9fea004d351827b16910619f8b7eafd5f098f72eeb9fbabddfc0b4e08f89a9c163a69ed" },
                { "gd", "7ec18bd1729d093e4ed6a8b34ee7445a282ba89a766ea76cc86eb655459eccab6dd4522c5a1ad5a0d335c6b3668881b42debbb81f1b9cf5527ca2cbf04fd4611" },
                { "gl", "a1b27aaa1f8b8d542b1a4d7ca3bb401b1dffbedebca0804c20da92168b535e03db45c703ccee72bbae3fdbc55f744fa8dd39f133db695bd439c2ba482d8f92b7" },
                { "gn", "3c04d7e2191f8ce008933b3902bc8acaf0fadc26ff3b31b0537fc77023f72dc4500227af64663f5a5a375013cf58042691d4f0aed45c86cf15ceb73929bd8eca" },
                { "gu-IN", "89433935f8f91fdfab402015b99c2da1b8a1384e44373d2c4319c9c417b1312aa9ea2be7fa49390343f932c90737fa2c0693fa64d1a3c6fa87dc202d286b79f9" },
                { "he", "3c815fa0ceef9743f3ae4f9a2baf9475ce941f024b8b6995bf088c539b1e9accffd6a31590132d80b3a762cd9e3380a8787545da0b1d2197a3067079aecc901e" },
                { "hi-IN", "c1515ad141d3f805256664835ad40674aa28519251d4a853fc8bc06ed139a150b822e50858a89adf835d3eb3873547628906f934604ef6929c2b171174fdc0d6" },
                { "hr", "e2c5d0f47f84ef4dd679135f9e8c8ff3448934c33f55ae8ba589d23f3bb0fdbeb87196bfd71b23856fee9061f78ba8af985b3880f49ae050c35e396137c30eb9" },
                { "hsb", "ddcd9d1793503f85210182d3727c35dc19681d54508a461d0b610a0fd37fe23233276ea4a365d9d2438a993878d592e2e9a801f82f7cb612427fee4a288547d3" },
                { "hu", "47bdea361498b7aeb5b8273dbe8c89397a6d3557072058a402ca43e242489fd72190b5992ceb206e3464ae9c33910edbcadff8a7898c16209e9ddbd73db4f248" },
                { "hy-AM", "f8d2546c5e345bbeff40f6c9bb35ca1297b2a6654e0a14d820c548fe75fe8e7cda796ad5df0ead6131552b46a538af12fa7d6f844add0d5b16625a5854161163" },
                { "ia", "4c3e9e2d49ffec8adc27fd704b6936a83f53e554d6c711bd53e2a173d8741d4a3b566d3e12e31dbb92792a2bd5706b5345108957292763972558d27818ffaf00" },
                { "id", "f64599555fe6b345b4469153547ee12f83dd3fb67dfe989e985407c2959c761f6cec54a2812113a54325bbfc6f2e2ebfe7e35e553325ac6d55762a880c6a7097" },
                { "is", "7528d85c212b1b2f727dbbe319a73b60160347804007872c85e6ac477f75c6da26ebe0b73547952f7a1e65035b99a3c093aaceacc5fa032a45e703b93b48789f" },
                { "it", "4ca31318f0e8c5df89fa839d7d4f1c291bd0d7699967ab62abb0d52ff03d85bcc03cb3faba43c14829648bd4210f2088951c5615a1112599d093a724a107006a" },
                { "ja", "03e3db13960a9dae592e4b2af7f05eeac4c0c338cf5a2435667ec2725911a54c37dd3f9aae7502377ce19c7f959a20009169141802465fd8a4a2caedd6d37199" },
                { "ka", "bbdda1d2c865be618508aae8fa42c745ce1c8c305f2f782a7851843a7a5e8858435379c640bbdaebf9eb031d069a705cbeca1532d737bd8fe149d513fbcd0abc" },
                { "kab", "dcc8c4515dbe5d99a71e57832a8d79c4a60bbeabf64693a49201a6ff4a10427db2f5a05c835235e2ac7a9e14367b19df94a695c0209c1885307b72b541097a5a" },
                { "kk", "a8b174aeb371856b9b0de41f75c30c86552959e9fad0f0acbd590e0e3ef664144c008b574d42705845c9b44365851727ee121c1127221d8e9e240da3b460f809" },
                { "km", "ac6e9f9723a8a158b59c9ffe32d2527aa260b50aa20da2f5a2cb852702d7c48fc9d2c2c408e53370e03c74be0f2d2bae75b4f1f2e157474f1a280195821dd996" },
                { "kn", "a77334bf9b68e6fac24b290eb29d87bb947611383dac915495f413744801eca50db76a8bbf31d278b04a38529d7dc14a45aeaa855ff00f43f233b755351bee78" },
                { "ko", "7869a6c0330339264edfe21f3e4d2ef731a9eab937b995cdfd11088f5e4fb64795c01d811f27ef8f218fc73568ea4c6be6bbdbe7b94cd47a785c3b93e41df81d" },
                { "lij", "ab46e4c9e1b637730ba633b06a738a15cabe195d7d3d8a7d6877a7ca0eacb8fdf9fcebaba3809ed3a6a683605695677e8433efb3d713757c11d2802fa0f8cb50" },
                { "lt", "03c014dca8e2015730a7e22bca8640587d3c59508e6abfaf9a34f5a9dfc9a6b85588b164d8d3ae02941a4164af1ba297177cbaab40672a28efca3e99e4648b10" },
                { "lv", "a3033985848763dff19219b1b79b7ce19045b09fe1983230228eda0df287097756c54b00573f68d49394ee0c2657521265eccfb5f3ea9e42cc264a9bf1cb597e" },
                { "mk", "5fc7dba50d3699867318198380eba5d1e09150edd833eece338ebde5984a1d5a00030bfa11b8e7bcabe4ad44697a607af5d75302fe5f20d4c12adf7c00592155" },
                { "mr", "baecd8ea76858cc508be1909df1c0dd72ada538530f4676302d31832d359bc5d80da2fb4701a580208f0e1be7f72d81e7c84bf61dd8e2bf8f922aa01677c80c1" },
                { "ms", "bc9c465de55aa428199b51050728bb90aa7ec8ef9da7934ffb77ecb10bf75a61d4c14c1bedd382af52c948fc1438d053c9a244cc0a55dcf6fac4d942a5014747" },
                { "my", "6a735023a7ed1f0e8052f3386305fbc07905582ea36a55152c6337b50f3321c6da09fc3fd2c3696e35d4a73a6ffff82be75c2cd0e9088d00eb2e3ade76114762" },
                { "nb-NO", "8747a954e264eefd3e31c5c15df34358d109673c990c04d08592a638f34e4b55b986c45c978517a1f11df8ca0988317c2af7a08548d26fc2271fcbf3cbcaeb1e" },
                { "ne-NP", "b50ecc399027e567709fc647399353d7699e9e7956e59ec9adef25aca3f1ab7d04a0799cb3c3220b9ee835cb4a22ddc2ff4d6409657271af93f79900a57cabcf" },
                { "nl", "f6a63f06a7151ada8c70fe50276e1244c5428910128ce8c6b8d1e92c757c6536407ccbf599710f1e2a9b140a3e1579f657a447a8db3e12d95c203d5f2c13ef66" },
                { "nn-NO", "b6e35e06f1481982d08bf7265254375701865dd971440a10e8f87e19895faa2b065aed9398c6f7b201f11dd0ddd0981c87345b734f83a64a5208c9336e0150a9" },
                { "oc", "d2ab3d15208c3ff970904dafa07944929a1bd053a6a91886bc68f21a9f8520e2e6c7f5091ae76e4b939501c6272f67799085a399bff69050e34a549aee87add1" },
                { "pa-IN", "d57e349d1e2dcb7549d8956d04c4dc5da2770b28a6847328bcc824d00b84066032af53ec10b374b0c95fbf6e9ca87ab7df630e8b7304b5f8722daa0f9394f985" },
                { "pl", "4421c953a59eed47a80ad0eca364dfe349a11e668053ab0723949915f3731eb226ded524751878ca4a8b6120c7414ff5be863b1bc4a509163d5f36eacdae2709" },
                { "pt-BR", "fa972bf18c61c242e281580d4317c2f27db7a65175f65cb91f0086bed4ccf58a3c6209ebc1e3fac195ae62ec00e19ad0a283a14c1e461f7bdaeeee698546d98d" },
                { "pt-PT", "ea5e374f684acd98117b8504cee88931f88e3b0e5d005a28746108cfe8dbe44751150db4ef2fba68901e81e6b6a5498a764fc57185909ab2c4c4327749fb4254" },
                { "rm", "3a0145554598dca37d649a43f49e456660cc80b76c1b0cf78bbb4fa19d1785303ab8e914d236f14e272ebd3b0137c29fef9057394cd1aceadc3d8e1f289cd527" },
                { "ro", "29b6258b8f761deba6f4bd127b0527dd0eb2e94b2eeafe1339d203730b645e9d10601fba5309d99f0208a1787b850a2dfc976bd4added77c3951934f9ed8751b" },
                { "ru", "c4f5f7979dbef8ec6fa703e382f68f4f9fd17193af0bd503194d9599beb67963ed0ec2ed5ceaacc7ad60f94c4779ee780f36215ffaa289e4c3a6d3f59a27dd66" },
                { "si", "97ba6d8216b03112d9cb96888e34ea3ec810dcac70014bb3574e64047ee60cf221a99e5346b77b970a6b31c9f49c3726632f5c76abcd4c0b4765b4df164eca88" },
                { "sk", "1dabf6912f3eb3cde02f831cc8f2e60dba0f3be8545a4a95cf03a5b5051184cc076545ed541c74bd467080cf503503b6e0a740aef7fae5fe66d6555f58139a68" },
                { "sl", "09ddaa728f9d71437f0969f6ee6cf2883e6a1472d0f45f325cf52fb0025d2de97dc350db673eb8c6e33d930fb57c5db30a48f844ca603ed9bf9e8e734e3ad2a6" },
                { "son", "f91b94bc15c31a80bb41d57032dedf07496df361b42401f75214e1c306706e6e8408a6812c0753860677399504a5435e4b294c40cef9d7fa5c66804d15d1c00a" },
                { "sq", "63a03594194c0ede6e0131f80a22136bc3c4973d8bf9f8f71269db6c155dba88233d54c52d765305fccc9dd4068ae5bc270e25f6d5192b45c009cd4ccf0f2d72" },
                { "sr", "c29f348ccfb0fbdc8ce89f90b738e1ddfb024e4dfb7305507f5e224f962db8dad28df7dce21819b06cc62727312da0cff39a0558b18adc003d9b7f8025b39a43" },
                { "sv-SE", "67129de3b51c69283ae31d3ed34ed89836a8358f120b057e669cb985d0c0cbf36796a99a8ab1cf6fa55face2ed314f6f4861467e22eb7db61c097cbb653077eb" },
                { "ta", "3d8c62c7952b6a1fc5cc6141a5604bdc6ea39240c457399175c6b6db5f571311beff68242a3841b0895b0b4537d0ddcc5e705072fe379265f56c2ddf44ff5cde" },
                { "te", "6415a9b3e64060b8fac9faff5b09ae4b18aea3446513981499311a71c603b0e032fc44bc0ccc65af20b3196b79a694150a4cb02a7391c1ae89549f27048cd6a3" },
                { "th", "3df0b8b424d50c96872a6f2bd43afa819785febc997dbc7bf76593cd74afe059f537b0547e1404a1ea3762f2567b99e91cd047f25374b38fe3aeda2afea5e993" },
                { "tl", "c23452bedec494542d3de7115aaca8429e3a7b15e1af7c42195068a223f6279d4559ff356bedb0ccea5eaa1d171973d2127a9ab8d223f37ca5176eda9556b8c7" },
                { "tr", "a86d3bc9bbaa226ae964aed08a72827d2a5f098178ba23169f010cc7244257a7259870053996d2f3f5647012981c3cdbd5a096fd1fbef5caf636b555945594db" },
                { "trs", "73fb22f4e6aff66f080e3ae5acab542372af43c20799836b520d0981eb0b11d43aa7a2381caed3f853020762498eed8b204ac3c8e851086f85b6eefc51947a9b" },
                { "uk", "4c2dd0395244ab9d3732b44d0857608bde9e4e893e10cf145d58f9bf70989494f5ddf098a04e3e6ce879f62a468ff1653d35bd9907be449200cd6e6b55a8bf06" },
                { "ur", "28fe02ee2e3db18f0dec6220a0e077b4a56c077541555cde1d88062f83923b549b28e2dddef65ce27f3029f292abe7b2980c5aa73a7f64c17bc797159f921207" },
                { "uz", "b56ad20e5c1e324420e21e3d8832361bbb8d82be9d7e6753829be432a11c67b6353cc5701d9b95afb212268370e0dd0466449cb02115ab2326521601ce6e0b55" },
                { "vi", "643718b8bbc32c6277361f61c91afb6991ed6c2b36a419e25754b567967b90d4308526f4ecd3c498f511df192e713af3f089d3aed6db646b04651a18106a341d" },
                { "xh", "35e97cf3c02ff5512d533feee9f2e7846f148c2ecc915e9878b62b50bf227af1de3fe3cdfc023474bc4d88bf0225900906574ca9a7c4f238faf65c9f767ca860" },
                { "zh-CN", "60808af0e16676b141968d01289cec9bf647a2f4ac856ecb3c772fc3f8122f1af2d0f9cc60f1e6fc715ab1cf6d019991bdfe440b0afc123691012ddfff6fb838" },
                { "zh-TW", "4c46fcfda114a9a7a8b2bf93435fee4247c43d74e754383aee7a923e9b46ff824fb5317c14892fc7b9bce61183bc5eb47fce4c2c853dd23df3fd09d0bfc9a982" }
            };
        }


        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/firefox/releases/78.15.0esr/SHA512SUMS
            return new Dictionary<string, string>(95)
            {
                { "ach", "320451221f388e95f244e50afbdb806fa78188bac83b02a05d1ecc3368d79114441c7ac90eb9bf39e7a1b0928e8d1904420c7e65d31e8d788cb1b96ed02c28c9" },
                { "af", "afd3d7775c0f8797940043558a8ec745780e7262377ac317ed524d7f288ce66017f81ad044daa2457fc83769b5e24a646a670163c36012db9cc5d2b97dac0050" },
                { "an", "c7131d5fd62a4751f1fe12a4ee24aa8f7e6e89b9d395f519601769c68ae6f7c35f55168d57387049d3933ec8c6a136661ad989fea7977e7a7261bbb6d762a08b" },
                { "ar", "0a2d8cec9bb1dad1bc235d57aa9f2ee25e2fad932e599550a65b09d963ef889281822e3dcb40d70487dfac19f1327e218a0eea349902cb4e714ec937a6e7afb7" },
                { "ast", "5508763ab2fc55438462624c307acfe419092df9ae0073fdee4158ee010d995c677d1ce5a0917329ec2c28f7cc56e4b9cf340b6ee1db0bd37170dc45285fe16d" },
                { "az", "dd83dc873de0861ae61909f3218720c93d02026e2e0aa4da7fe731bc2d177283bfbd6eddc688802021e1c1fe2084133af810535c3418dcb2a5e5b17a74133f92" },
                { "be", "c1d7519408cd3eb144b7c516525016a320670248fa3ed21066d75c3e97337c22b7acaa16d35f0a24731f4ee4a0692ae78174282e1719b2d2abf1f44769860cba" },
                { "bg", "1f561030e991630e8f17d750b1434344fefebf62997166f9f60f7ec6f5c5ae0eecdf4eb1b1f58e7f801796ea38c0ccf7eb011a44f36fa29ab27f99fd4f54196b" },
                { "bn", "ff75024cbc21bfdf81e5ddb40797858ad4d9c59ae0a7b7f2468114fd2840458574993a53d2e18d7fb9c47462c219faacc006a28fc7508dd0a980b7cf77450a99" },
                { "br", "c9face2ede3a64fa68e92df386111d91c180e6b139598b1a54aa5c5dd38598784151d3983061823bf570c5ab71597e125584ceebfc6139b72d34462a5b0fc690" },
                { "bs", "b0b3c586cc75d296fa89f5655c5551049aa9e6c8850badd46782244aff835e343d588ce7dad774383c5d03300c0b60ce7c342fddae868b64ad9e60c9a2e8ff7b" },
                { "ca", "b16f813c2479f78bb7dd645fb38aa7d3ddf097f677f5c23a83a1d310fb5ded6fb0f5645ab82fe7fbee8ed6a8336a521ab13b1527cc6bf8eecb43af5f41775ec2" },
                { "cak", "3285b0be58f70411aaf6b00910500dea5bdedc1a60b3c1197252eaadeed71f3c5a7f38ed89a4291041a36bbc2e6797c5d6925a367296cbb0e1cefb71f26049d3" },
                { "cs", "8e557cbac81aaef94092ea8474e8bdb1d1d6e81f6ff238f943e1e854bb0454cff854ab867dc88c9dfc25c9b6b1e935c1dc2de4a275fadf72c0dab1e841989000" },
                { "cy", "de645aec263ccfabdfe93917bda9fce5adf5116ebc03a40dfaf4bb9a3df3dae3d46f0f64792274a7eb3b85521d922eabbff6f1500a6cbb9b5e0fb166f9e0d2a9" },
                { "da", "19cec8023026ec62953266831da5e313bff14cce35de1a8c15ba33bb259e899335a7f144a4ef6f67c55a705f5bfb14fa1e13ab8df92c55c4ad23cc04b2112d55" },
                { "de", "431ddd35362678b65f72dcdcb5b737ec0ce6ad9fef3754bd2e3d076394f41abf41f78e840debbe6eac7e451533ecbec137f2e4ec94bfb0da2cf695403984ffde" },
                { "dsb", "c4bbab667a4291239ffa6156715ac9cccc5cdda8e071da84b8dd913c53222942b3e8e526119465371b2e7a254039ffb6a7f310ac0e7e2ba57772c796b92d3d85" },
                { "el", "3509325323a8bae3b494e9ceb0a5e7df504a9576be03427da1344444825080d19a46883c4fee3fc71dff1fecc720c5eda02de6c119c3894c9884e4cc97285b0b" },
                { "en-CA", "928cbed838a570acafd8701eca94a136e2da01839e62a7fee9ed178267e036c6d793f1a371fc8d0d300e0e17e5f636daab3396e921c0896290a12a1f1ceee974" },
                { "en-GB", "0602120eab3dc5cf0cce167e82cc8a0ee73a6fa297ce974cc2436079726bf0e82586fd92d4e24fb2a1e2710d863dd9267b361a2492124a0052f4ca5762e2b4ad" },
                { "en-US", "3438c8a95bd02c9f2891ecab8efe644ab4f05dd543bfbc8c12cdae491d058599c86eb4aa9d19cd203bac90c6eb2028c895422ceccbb7f197d468e0e6c9a51f44" },
                { "eo", "577267fa9796aeab619c5269466258d31a78cf3e8ccfa09a1ac78b0c406c8299f0b575eed4f6d6c4fb33748c332cfecdd97099a4211f1ba4cb4c12ec49abbff0" },
                { "es-AR", "ad6ec54e5d09b34a796ee00eeb38a8c27d6c9ccdc6d966061aa36115e7500e855b91b0e991aa143ee5dbc2dc44b6b41c7c273159a341a44e6e0d3c82fd779e2a" },
                { "es-CL", "f8323449548de3526c5e7bde4d3eb34119816dd5afe76cd1a8cc4a68cca915cedf7d8dc85a07b867554ad76cd6f7e3bba80f40f7c609959278466333657e0bd7" },
                { "es-ES", "d0bd3ce1d6ecbb33b2c3b7fa7910b3067b08bfb571c7908d132b550019cbf6809459b1d8d59a8622d5b9aca273cc27eec1b582612783a3c5daf3ac59221d15ca" },
                { "es-MX", "974f72ec4e6f27d93abbbcd25bac4d4027c0a799c65c343918ca85bfb542f645b0fc2f0bb61de4eb87b363154fdb5098e639ea6a99145c17cee408b689410f8a" },
                { "et", "69d6cb0c2a83b47e1872cdc868d008e153c8e98a756b644413f52d984d35229c24262df2facac108f5542d5c600d9b1d1032c9bcc91498c647a42e640d80f1ce" },
                { "eu", "8416696dbe83ee56b36acbc7aba67b9e81dd887e097688a3fdb7a9e5273df6a48193dfa6cc9dfd4990cc639b01cd8799a71f8c290997b7027564387c8e679deb" },
                { "fa", "e2e6221999911d895d6ecfdb5666d22ff76eafb8fe693aaadc6958d87066bb5d9f2cb144050b237aef2b5dfb5f40b6ead16c233c5f8432bc1f38e252733a0fdb" },
                { "ff", "b00472ebf1236d90bbdeb0ae21ee9e8ef99bddaa848a14b69c07ee86929b71203c283c03f82b1105b69cf06b201e687965e175bc20ccf573d0ea2635c75ebed6" },
                { "fi", "f7e1a33e2abcebed2a8214903efd4b2a5bc633d92aa25e8cb749776f64d78d4b1202a6204bdb90b0141c34488a3a32d5d5935e7fd981afb8e61aba71bbbf3f92" },
                { "fr", "b906b7b747b57865983840159b3c128d56a374d60b46c196f051ebce469c2b321969df4b821d1630e7ff7ee09faf09275e71c2b51368ff0230e6b6506772e4ba" },
                { "fy-NL", "71cb7bbce7582224cd0638ae6fca086b2bd15d754200c761e97f15096e4532b3d6722205deab217dda0a5fd82f4a01f658d818ffb883a1c1e130291e658874d1" },
                { "ga-IE", "22db7d1a4d3c378ed1c61ecdea4b3f7b8805ddee9ed023e3bd76af06f86197fdb26574b160aab6838dd8e84d9fb00dddfc5d40420d5e2b072de1da5af61eeeb5" },
                { "gd", "20a35ae2abdf21fbd6c3c45e896a022e59505ef7c4e83a94432957de49c1eb2b6295eab41d6813752e6eb46271c6b3744c9cb58aa3222d9aa9c42cf01287c234" },
                { "gl", "457257a747a661de31950db68f25438bea056fc3b6565e7d7bd5085d2a1ebdd115b3078741b1f1ee7b649053c5bf170697f2384b12f314f3f94d80e9508ada63" },
                { "gn", "37699f27a52d0a45c38e408856564e3243b6315a9d4226547119d87a41878a9d06e158a9e11bbc0f1452513ede2c8c633d1f3494540018ceb2227f894a587acb" },
                { "gu-IN", "0257fd6448522ae01cba5078c18a58125e291a6ff17b41af4fec1fa20459cdc278eb3ed060a0f27ab2c863c0b9ebd2bae10a5753231a63e791c8f24c143aba8d" },
                { "he", "61a98e427428bbfcb826e89bb7bfb8e9197154f7da476695a302e0212bab07cc640b9b5ab648927f7f827d3d5c3311edada1e26b73bf51313e3bf74b0decfd3f" },
                { "hi-IN", "9d22cf2ba6415b68a1921fd3f64e9067258d180d206380485643d0f367805ea4504793b54f77de7ce919e987d3bc11e746eec247589410aa81f4bdfff49a15ed" },
                { "hr", "804b8a6dfd9a1324bef598a4b1dc16d186491f79a9ead897a5a2be8ae39bb4293ce1591a76ace11de07cbc2136c7e5b0101fe359cd14fe71a24a99e6da0df24c" },
                { "hsb", "2635a883ece9e967e315aafd87be2ae384ae3f36d142f474d1bf8395b1d003a130b921e6313923207a1450296e526fedbcfa237ac867eaf49a7f93513d5db395" },
                { "hu", "70bd23929361b6d6f481ee7a9ccf50eb9d4a2aa8d269ad440e3ce10b6adb9221f168f9e58f3a3b7e4dffc705999c6a0d3cc09da53fae3e82e861f96da040b6c3" },
                { "hy-AM", "2d09d2aadfe881949c7eface52101fd26c42ff1360f943f75b5f3541e7f7acdde64eab2aede0114465891097d3453b2a5d3b1933f902a9dd27b466d45a179748" },
                { "ia", "b6655ca50bf316b13a2cb0148292c5395d2485d8e6f5a99cac6ae61d0649be7de087292c094b2bb4bd27d9edd82de2f2a08d499f196031fa73da303a3c2fc236" },
                { "id", "4970335a599ed87f8e73604d3fd0ae2cccfeb0215270b1ec6236c31d6b1bb608bafeee3ebe351913518b85ad7e74eb4a631d854f29dde40e31e0141ad08076a3" },
                { "is", "10155e2af7e59c8a8152ac22404c69fd1e96dfe4636eb795cc70c3213cee4b3c32f12690f6f5c5324e1508b1b5713f3264bd18941fb45556c171129a0c6b112e" },
                { "it", "53f5509365ea3af20f70cec29822d6d5aeda2fba8ea9fc0522d50515343f1421d32f1f6f4dab22cf6d61d5afb59ed91694773f981540df59b14ddc578cfa0c36" },
                { "ja", "dac2f20537b7b5f1a354b11c6b2e1c9bf8a4e66f0138c99228b3c12ae36496ac161e71deadcdb999f493b61082d6690f32c198bc617732ae905449fbb5cb86d5" },
                { "ka", "af865262a5fd4937540f8317172b0cf16543b4743aa8cbd1b134fa8d00c6782d48d4cb742c464becd3e6b4034dd1770364d8efb27e8d43362a459c215462a702" },
                { "kab", "c5b20023dbb6144f571a1352463d547490b2f2a35c33d74dc497aa242ccfc6b3d24c0633f35074b817dd14603b7e263d7af81f70119f8d7a826bcee4b0e6d6a3" },
                { "kk", "02f8221ec1e09d4e9fd4e17199dcb89c3f4248c131a519ccb998d96c4a38f21a424d544b76d1d38863019c5917bdf5fcef991b544b20413eab9b6b5321686240" },
                { "km", "9e700f451a4945fb1891520058055685d00c681ee49ac61e4352298cbb07f66581a09e9e642e056b2e5e6426a61074c68b6c4b964a57f78221745d57a0352139" },
                { "kn", "3bef2e2453e6c48aded02a86e70c7b2e435935826013572a42dc7b2230a0af8e6482d2b68fa0184d102786dac1a1f37c4bd1ed9520bbe9bacbe5c14e6213d39b" },
                { "ko", "c7e22290b6fd2bd703c51cec28364523feadcc923db7dd2057a32516ea09b1e5cae7994490ee64a6e8bdf50371459cc686fb9475dc6609cf0a4175c8f00b0ff6" },
                { "lij", "3a74a1298fc0b0488513f0273b468396cae6a7b2f6eec874a1b0b056bfcd16cae02bd91f12c2256515862d6de44771e0a04dbbd75551ecf3740af510ecbb25fb" },
                { "lt", "ce31cb377c9d3ae3c2547ee181bb664b8c5249ddc914f1735a767a921abd4cbf37bfc628b993cafa678276d5e4228f8fa9030b92f8aace7b45b0da9e5dd33d70" },
                { "lv", "4fd2e732a456229c3d49fbfcafddffde2ff97eb6726d3ec751f203a76d3c9edfafb3097f87b3d8b099226de40f003d44ebdd09c0c2ed43d885d6a9243537aed6" },
                { "mk", "01680216acd903c8670b2e9d4cbbbdae6719a67f6dbde1d73d9142c49796d96c00b0f20f5d9fb43e2dbdb750435e49d95d41da3fd247031a7f6a9819670998bd" },
                { "mr", "b6897f205e396035d303f5b2d3cce9b944652cc91717910f893964559b37782eff004716118aeb9aaeac381fa32a89db090aad88f1d0de7f9cb373655e32f550" },
                { "ms", "dfab6a5a84a2d1873a185b1e4e7b87a724c69a44e501e12d9d2d1f1736f7837a670c768ce7e5641cfbd667a3acd82be900095ba7731b769322ef0408f3d9ac75" },
                { "my", "807e1a6f0862a1a865ed5f8adc47e271095e1f8fa51cf4b79ee932de669b6a3ec1d492bde992c0779aceb15e3b0d45d9683bd15c6479b2bb4161beff72f575c3" },
                { "nb-NO", "b3825a476c148dd74b41105a122d1704d436c9389b55824dec775872301e600399531911d2200c010e9c05a7a255135d25e70010920c1ee4e9ca731524e63c4a" },
                { "ne-NP", "2b25bc7f320fc8055eeb67746e75c375174cf2cc057c99ec94465195296229230fe8001398c3596d76e227fc694dd79919460a755cb4e58c1ffb1699971578f0" },
                { "nl", "24b4f4da564f6f486d6feedd8a8a7ba4d7bbc6c759f4e4cac825d7dba126ccc261f60e6c94d1da24c0d92087e7b15d5dec60df6a76b823c3a5c34ecb1e1b6704" },
                { "nn-NO", "53f322b1f1beabc3550391adde824689e7d2aee09c0a317ac5040238c98deb65538a4de11f8d8be17db6f8cfe29b05e31369ce2d2b8972626752134523cea1de" },
                { "oc", "f52f60f83a40f82b85c0137e7d935a285b58e1a6b61def1e42551e7b98ab0dbeb6c6f502d1b6ef17c66271d3d633af76e8ddc7a35e9dd0aad7b22086666d4d0f" },
                { "pa-IN", "61d639267e535a33002bb88946a3d3b1f35277f556613ea7b192852107850f1889e20773a3edd3deda8144503ff3e16177fafa9884c65d3d04241026f60be9ec" },
                { "pl", "94e1914e1fa5e07ded9611c25667bb82fbe3adf8790803b37c4c2803139348090ccaba99e9eaf5796c800ff408682fe30765eefd59f5940a8e86efe2d2240e48" },
                { "pt-BR", "2d387b2d45e58e383ef7bd6b6a96addcf533d168bf958ad178abf62194d2e89011d888b78d0996a6f2947878be63f5a78cc21e81a0dd01a2857470de62efa093" },
                { "pt-PT", "5e364dc2f188d338101a98f6f4dbb0ec2ad6d59d5121555eb07d19158a6513239a31f7f3abc833c1bc764743a7f04b271e3dbf12c6277191760ae2abda4d141f" },
                { "rm", "67d5ebf4391f5f731d12184d63604d61d89ca0916634b1b52bfad0d35b062dfeb381a58df2f88d38770cf9ff9f7a1749c2c3b6490cd86ad0a38e6977e93e8396" },
                { "ro", "7220de29665cb240be51f5358a9a6c8b2e8e92c878ec3a4de8ad47b5836dc5910b99dd023103dbf3a28c4b37e3a994cd38e87fc9a7d51943d61bcb1570316826" },
                { "ru", "d7ad852f86ae20021da63237d4ece3e3b16b70fea5d5cf06f2292bfcac8cf693156c12b134f9728aef9931455e61cd2bd2867ad6e89d4d50892bf4d2aa4f9ab6" },
                { "si", "6a8510e4e7ff61eba6b38ff9910cebaf1831bba525b51ef73d7cd3f1b89c671ba4a35f1e5db12139745fc95e68b58bd080e1023b4256801f1d0ec52f5e2e4010" },
                { "sk", "08f8ab473f3dbde3e88a0f9c5bc514a6e151b062c7fc7e786f47451c871140bf8ba454ad768e5d10dc1b821351d6cd36d016b560a437cda5407ea23ab9bac74d" },
                { "sl", "a281af78110a742abd4c660a78c499367f2fe8e06b1e268ac113445cf1a88370c4b8d659386668a98a86e2b91ccf0a1ac0880067076e4c26248dcfadbf46c98c" },
                { "son", "8df9a2ae1a3eea5cdc8764f49a977c730217716f7d170e59a00ce2366158d0853fe572cb60760d96ff4778fc2f183b9fa71c643fb5dfd76ce4502685d0976cbc" },
                { "sq", "6e6e1360f07d1135e13810c0baef3e03b1f50c83781323c9d48206510186e35a10a7847b0603c4ba298612949d59814fb2d14f9fb18e4e268e17160df4936c8e" },
                { "sr", "340cd6a8c0061ec9ee841a365fc1e4ffd17d2f78da4d6cfbc668bd0a937f594f171468d4b3102b8aa985225c6f5e48ee283276a27f7875a063c801005c723e45" },
                { "sv-SE", "7ef3424b8eb4ec18a4cf7466ca3a1901be3e39b697639d2bfdc8af8fd32672b550af7accf1c95b9048a16f69517294f7691b99920a18e1dc33b256d8ff09b5f2" },
                { "ta", "df26097c54120daedd714f00f8be8a85eab6035563732c3149646be6ab1d27d73a23c6198ed14b66fc08b455d10a29de5f4d6b42cebbd4a0552d0c0e827c87b2" },
                { "te", "edb6a7a09bd5a335f9763faa10c37d335f9920b618e31de240ee61ecd7c9ab128611f3d8f6b9ccd997e1d841404db19a5e297b669940a4f3ffbf197f0e720165" },
                { "th", "d9b8c47d3be39fc637f0d65bc2910b1a0ce751e81c264d23e4135f57b3287c44733aa8fd962f2b52bf54c37632f613604d677d533da895057e58e6cccbc4cfbf" },
                { "tl", "34471b087559632e33c2a2ca8c11dbc86f12a8fd14d19ec4357036206ac2005085dfee4b23d9762d4c3be1e11c27a6e024c3e6e721564fb6d45acff8bdb4c544" },
                { "tr", "4105cfbde2b1defc45f6ba02aed96465bb85ee2f94064bc5fa23db95ec830ff78c37a38c908f8917f6b19b86d2b3c88e8ee6db9aecd181fc93fac1589bc3d33b" },
                { "trs", "dd136049dcf88c984c94ee9cf39fd62c9f1ec4ed50e433d3c5fbec180aa7ded5c6050fa28819a22ea3d73b70cfeec588ba63eb093e7a490a7d5b14bb8ab771ee" },
                { "uk", "3ae6d6948b135b78925af226d6e31f802233102c195e69efbff6ca399ab02efa21a5bf8b2ed03ef2aa8adce8017e5c63117c4d184d9d3cbf360751e17f9e892d" },
                { "ur", "4fbe7756c4c5bdbda8a1cb1a2a49d85195836d197edfb2a7b1bd75b6ab3d96961cdfa0439076a7f7e0b58ae72fa24fbc855b0961825988b7ca076422dfd98969" },
                { "uz", "e4e161bd2ed3c143e74595bbdcdfb966d0a230051fbd79891a18714da5189ea50765a6d77eb082e93050fc9f4f9404863755483eaf7fb2ef3a83295b10c4931c" },
                { "vi", "65822c15105372b456785147537d8d000a9bde38ffb9f4e3e58de7d8b8d3882cafea493927415e8ed4b1ea0416036839164f75185cb800168dc312122da6b3c0" },
                { "xh", "4c62df80e918ac3b6d4f4971f061892b1a1f69288e3834a9fa7060e1b74e88c961aaa64df36516ab0b139f5069acaa4ad4e1e72b99f3b3b05af682596af41006" },
                { "zh-CN", "3bab74368d3aa5a8315886c95c5effa04b72d4595148358c0214fd5f00d366a273a5734a3191ba88d18e1ea258b6cb640e1e33e27d6c077bc61e922f550be7e5" },
                { "zh-TW", "0c214f3719306d5f03efce65b135cd74e22e701e65670cbc9ca170ee36c35fae2eaddebae5745869c914010034db0f6a250c0eb72c54c263b7037c8978a2717d" }
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
            const string knownVersion = "78.15.0";
            return new AvailableSoftware("Mozilla Firefox ESR (" + languageCode + ")",
                knownVersion,
                "^Mozilla Firefox( [0-9]{2}\\.[0-9]+(\\.[0-9]+)?)? ESR \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Mozilla Firefox( [0-9]{2}\\.[0-9]+(\\.[0-9]+)?)? ESR \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win32/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    "https://ftp.mozilla.org/pub/firefox/releases/" + knownVersion + "esr/win64/" + languageCode + "/Firefox%20Setup%20" + knownVersion + "esr.exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-esr", "firefox-esr-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox ESR.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://download.mozilla.org/?product=firefox-esr-latest&os=win&lang=" + languageCode;
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
                return matchVersion.Value;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox ESR version: " + ex.Message);
                return null;
            }
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/firefox/releases/45.7.0esr/SHA512SUMS
             * Common lines look like
             * "a59849ff...6761  win32/en-GB/Firefox Setup 45.7.0esr.exe"
             */

            string url = "https://ftp.mozilla.org/pub/firefox/releases/" + newerVersion + "esr/SHA512SUMS";
            string sha512SumsContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    sha512SumsContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer version of Firefox ESR: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using
            // look for line with the correct language code and version for 32 bit
            Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum32Bit = reChecksum32Bit.Match(sha512SumsContent);
            if (!matchChecksum32Bit.Success)
                return null;
            // look for line with the correct language code and version for 64 bit
            Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/" + languageCode.Replace("-", "\\-")
                + "/Firefox Setup " + Regex.Escape(newerVersion) + "esr\\.exe");
            Match matchChecksum64Bit = reChecksum64Bit.Match(sha512SumsContent);
            if (!matchChecksum64Bit.Success)
                return null;
            // Checksum is the first 128 characters of the match.
            return new string[] { matchChecksum32Bit.Value.Substring(0, 128), matchChecksum64Bit.Value.Substring(0, 128) };
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            // Firefox ESR can be updated, even while it is running, so there
            // is no need to list firefox.exe here.
            return new List<string>();
        }


        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
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
            logger.Info("Searching for newer version of Firefox ESR (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            var newTriple = new versions.Triple(newerVersion);
            var currentTriple = new versions.Triple(currentInfo.newestVersion);
            if (newerVersion == currentInfo.newestVersion || newTriple < currentTriple)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
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
        /// language code for the Firefox ESR version
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
