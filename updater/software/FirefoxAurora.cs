﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022  Dirk Stolle

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
using System.Linq;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "106.0b8";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "dd15ae1340c2f84e310eb30b7433fd38129741858013b1bd8779de75153ad0f841c97112f78e49ddb3902961fddf35b8eb000b2f6b4ac6c2372d16644b320a9f" },
                { "af", "e1a9d41072dca0d69981f9f74adc3d9c655212a29b7347ba325efa889650956ec528481881a9a1a2edb09f45f5601c9ec5f4f345e359aa305dcddef79c3df37a" },
                { "an", "88f4f6673d0d8fb55d946edb416300fe770988f93a6403f5afb863a96e90572ded0ba545562f4e33c56616c2f7bd0e855288b678401a57c74353e527efd2a89a" },
                { "ar", "2665b4d20783eeb372ec35a39e71474d9a161f5088b90f07b24d19ce868fa1acc04280e53b7760eb9fbe268195bb32a6e61d996d81acda87a8ac2916ea18758e" },
                { "ast", "b21732c2a104d2b99ce270824c61426639e54957bb85b88021b40ac7eeaed7c5e2c41d93e136c73cf455cf397c74645c53426f699e6ca28e1773dca0332206bf" },
                { "az", "d8093029fc9fb77b0df5d94ed011156263de4d0b61f36f1e183c0e50457e80969119ce9e48ae92c68e999791e0abb0aee1c8df66ef5a126f6b17c49a6e6ed593" },
                { "be", "5b61e6cf677214b0a7bccdd2f6d73780265cfb4eba21a7dc1a79b7f5b8908d1b674246a9462d40787f2c0e0f554f08c19c89cc82ab0baadc8a7669f143ee4df5" },
                { "bg", "9d26bd5c72ec5d22ceb8973ffe1405f6bd1f75f437796de7d384980b99cf2757c9e5eb6ffef9fda6b85dba98f45b8596e61e6dd9bba6f2d6da8caa6bfb634d9e" },
                { "bn", "62995e3d02dd910b14897d6009ff6d334baeaa5677dc207e29ed04e4c45e05c565060b7a3d786d8164558e27203c44511c6038bdd157fc22b475194da0210d7d" },
                { "br", "4ebffb228eb9bf1aa3b7618f1c1093ebc3c68025f0a5ed29de44141df26cd66ae0c7d70410fcc3f8dbfb1af415ed6876d30e34df3b64c5ecf467df68c35e6e6c" },
                { "bs", "085274ba19cb6828d2070cbb43a21779d9b16454f682d87cfe6772561410cad40642417614b65d823405b1bfa4fe77e2c44684aafc2bf4a0d1439467403b9445" },
                { "ca", "eedbb6a2100753b547a31227ef39579afc826eca522cc77eee9a2560e56dd873313f679f1820073d5bd688ff574fd33242eb3e497b0e7517647182b2eb0e65ff" },
                { "cak", "bafbe980a58e05e96219262ef6ff6ba22ddb08d580f8798cd46920a3c9442ef52d747697c217b4d3f46db1164b9b7d652d2e99dc87e946ba5bf6eac2896872c0" },
                { "cs", "03becdbfb62e5ec04284a0010da2c3173cc65f59312167861e868b887e9467cd3dd9cdb447395058e1b5b4913084b9700f8d3d97849762ef65b0752e438af91c" },
                { "cy", "6351e18a077dbe1a1fbb42e40db278267f8ea85342c0b81b744f81a85c8fbdfdaefafb9cf58204d3a1b2f426706d8f86fb205219f9ec4daaf4a71939c56b0e2e" },
                { "da", "a68f4e785e33b9552e3d8002d573213570928a9b97bee91eaa0cc83cc939a78d10e9fb6cb33e64949bd0dc9c9eb9b26e8e5094b5a216e6e7bd05eccc5ccac179" },
                { "de", "09151185c66a34fc189abd902f81ea0f16b27da6c5d033bccbe3db28870463545ced1d5c425f0fb82cb2785a910b8b16088ff91206302ab97cf914ba606b3ba1" },
                { "dsb", "13c00d4b9b00a57005dc9b325e7bf3c7f3d2e19fff377fa26e42d6b52279bd12e0534a851595e4867b4440f44a5b3ed4ada1d335e03247a5dc19299edb1581a0" },
                { "el", "72569e23159d833dbde27ecb23f0ce68cfd82c82872a00f210af523835d93fba66983b7a2a7a3cf679ee01dfd0f86907746d664d89670519e4d076f4d7c75d03" },
                { "en-CA", "acf891f10f492c057246eefff1bb2203046f9c94d3900c8a6d8a71641b823a372b7fd4dd8e8064768c8284782480a885a19fe773d616c649451e42001a88abe9" },
                { "en-GB", "8936a945f7ce5622c32ffd1c0977d6441272135cd6482fa60de19d3995f8b7bb3df36d02d194c61269ea83ae653fd42999a6989ac26bda96fcb2bbb5dc039df5" },
                { "en-US", "d45a0524f1059a8ebdd3b81a6fd63783a1070b793053b2d8a339098db243a44a91a91ba115fd1f07545d46d14fe830efd2a362f79cbb9d2a58cca12aefafd3f1" },
                { "eo", "8ec84cf5b0965268946e02dae1a8602a008280d62f4680b275b481ab0caf9fd1b09bb3c829f79d77482e8a7e72d7a938726d73afff7ac27db4961532ba968388" },
                { "es-AR", "2ec10b3044845dad2b721b951066d9027c0ee23a229c7be61f965e616dce4f537b9f43f9f8a85372f241e3bd4924a8b1752103a82bd4b6a8148caae14a03cd84" },
                { "es-CL", "2b62f69d41f00b3d93d242c07ced0a354da43d9eaccb38dac767e3ec15b68992053bed68a353cbcbfbb3e6ac8dccc531060537af6162ae5c024be71c18529fa5" },
                { "es-ES", "8e7f26f26d193ec8bc7d3288afce2a4ae87016d32a0929a33c7a27516382e16b0fd8af8009b00397d6a54f68949ac76834a1778eb564d509fee5d932663dd022" },
                { "es-MX", "507d7a9fb547a4defbbeb740d924ab101ffdd391f3310483fa5280bb9b6e964cfd8d723128d6223fafd8a6538173f012be20284bc223ddab47d25bc22dddb07f" },
                { "et", "e790d7c72b2fcb2dcac2946eff22151ad9693f13651abe7c075cbf8a8eba4b556f7493586a609ecbb2c9f3d5ae4045c9bd25677c0fcd18307359b4f289ed1298" },
                { "eu", "fcdfad0ee4a090569133f09209967bae65253fca233410290deed9babec36935a982be2468d2ffbc7f630b3b5f8e54ae8e59e4d3ecbc23c47011a26b68ce3769" },
                { "fa", "55676705b96c167fc7403d1e971ed505e25d93c3ba93e0a7e58db1a2755c8c61e8caccc2e5d382a2fbcb481e6086e9f7312ed78eece647fd480c18641a245379" },
                { "ff", "409c03a9fee41f42d3ada890bda4b8ee9b1cda192a21ee91afa866bc6da2d10935d0e5d5191ede575b8070802348b43496e569b6bc6f78e4bb81902307d06755" },
                { "fi", "85a04c03ebcc9021bcab49bcc826052d63ae2655d0b9cfe21e51bcb285910132940ffcccd2994fe8cd4a9325fd1025f88f923a7fa629f8ba244b7f9b1ba15028" },
                { "fr", "a3c2c1bc5574205e0bd0b9bfbb0f25b8da1c02b0d17f835aeca4442c53f8d35f2fa36ba021bbe97b94acf8e63bbb75aaa54ecd0fae8efe75eb4fd303f880e9e3" },
                { "fy-NL", "0fd717ad5860d1a6b76a7b4a297627b9b32f4ddb1f69722f6831a5319d06002d0904514947b755b3b5b51373ed5e68693f5fa74bedd82bbb335ee9881ec7d8e7" },
                { "ga-IE", "a98a11c5502c26b7bf6782207422ed99399c9adb925469d0baec84eaf94f6fe22e4b519bdcbfe1edcd351204597684f201e0bb0715e498076933d84a1e640bd4" },
                { "gd", "fd68491193388bc260a1a927d33cdab09d7c42ada5776f41d9d266aa911252b71ec68cd41cff850c379301df9321ff50c566b6620ce63b33e0ce0b8bc4099123" },
                { "gl", "11f1ec5e3b0449c3318ad035c879c6a342d56480bb55f0d459c6b467b7ce27128f36beccfd97023209477c262a0b3260bc902ddc78aeb807c2b1a897291fc70d" },
                { "gn", "91a67d77d37fd5e7a1a42da1c4d408cf3c4cf8f89d0ea3f2c6d64355c90e8e8ee8c714aa668467b7a55b8baff2f99798b6242fbd569d6f3df2ef8b0db70cf8b9" },
                { "gu-IN", "dc9193b89677fbc19fb29a2aa47d5ff3eafba9bea4603e2ec3f28184df9fced3cfea66bc60691298c26414c5cda7ecc1a8e5d3df645194bc69f4a111b7526492" },
                { "he", "748acccc46d9ae1684200a7f0eb8a2457c1caae705307d7add50cb43f7bcf37f49c5ef154ecb87dd8f43afaced475485766d9e4eba6da430877b5cdcf1a76568" },
                { "hi-IN", "8b7a0508f629e90ba84d252b0fbc23c1d52f506e9e1ac2d040d909dca019b72ab588e968081dc5bb864de57b9e4ef3d75571f8d341b5e75ef4fbfa0223c09122" },
                { "hr", "eb1cfd11c673d6dd43bd411c0e3c84c7ec59ef4f2ad9c594ec3b3f2e2ee324b80753dd9ff04a4ede93cf4d068ab0c00dcf293f8a986f2abb8d9c7f06a5ab0e13" },
                { "hsb", "fffeefadb9c8fae985369a49bd30b6c97662573e550e9ab4d4f07ed41c8235dda52e059f74d99bec7f253737a541bde4b9d990b307d29723ba923d5a4675d9a8" },
                { "hu", "e97126e921d50da96434e77c773b698d3f471f2b343b45630335ff95aa3870a140e448282529848235e27077833bec8bc78c01d07e215b19a70654bf37e832db" },
                { "hy-AM", "b845104b033263591cbc99b17904da90eea4eba768f8240c2b683a99c287727c2e04f77cc86ad822b86206eb7141b44f65e85344ffd1d8545ecab929f3950fda" },
                { "ia", "271a340b63b1b295241abf313bf7f06fcf915f501e45df0492f63a4865cce51b283dbcdc357e5cac36ff604931229beb103403d2628b9d68acbdec1374f34562" },
                { "id", "86c6b41d154d7168752c536ea3ddabf8f7afde6fd81e66ac4e04f7e6faa08c491673ba946a3cb88a9fa63652ee0fca5e83179f27985428f4af293863900dcf89" },
                { "is", "24202024e2853f35fb9d9167f265e5a2db8b2b2835662fd8dd2cd87da9fa576c952c919f1eb1a6b244d87d6d487375f4f06d49cb987251712f347202865a1f92" },
                { "it", "ea93da1e3ffcb8bba945999714b5c36bc1d6a80ccc1a67788b6b1adc07aad3ac5362af96de78df42620836903cc5135677ea2ff4d2a9e56a1a6b7720b691ae1e" },
                { "ja", "b9dfd908c6e1e059b0dbaa45bc842a82bf2e3cdb6ded6f11b931eb7cf6afeeafdf253d63c88ff3b2a4741d0a0399e25ec385cf1ee5068d74fafa24fdef7794db" },
                { "ka", "8c4ef93fc7d118be448bff4dd4362e19932beb4379c99115ff6c48eae44bc30d62359bd8d84f890876059c308e0a2fea014febc8d8da2953934bfe7965d18a31" },
                { "kab", "92d017350727bcbc8dbe9e4437a2534286f668a389fb81262e75dacd7263a57bd9169568b5e9ffd5a0753b20f172e474d0bf206e26b7eb0a583b7c6cf44d6eb0" },
                { "kk", "b88c335e079199ddf403b7b7d4ec4a2fa152e0ea0d055df9809d466515a234b546620bcd3b8f60123d3981e7fe04cb71b9d888f10527292165a239c7853c0214" },
                { "km", "78afb4ff29a33de86b09191fc0512044e967d70ae6dde52283328dd657065d5494310202b3b7cddabb5360304742ccc7a14ea9787648ab9a74dfa4b5ee4bafa3" },
                { "kn", "ff3eafe06d7234a6fa38bb2dc81eb36707d11fee6f180c23eb601ad0a03ae08282dd6c3288932381741928bcf4a2bcf82e00d486090a2c75ab932557a560b22f" },
                { "ko", "3e992ef04215e0fa7e2aa8d45af85bc2a7a71dcbfeb2c581d1f361dcbba4fa29afc809f22043cffe952fafd533515451c2a92373a2d19951caf3d9439d7bb42b" },
                { "lij", "54b6d350659a8098be2ead14fe190dd4d710cff7fefe8c23ec2277347303b9de2f51bbe72b82b17035aefd8dd0a6c44e42797554c7b60d6b8618d7bfc0bd00a8" },
                { "lt", "4c336b2fbd2f546ec6ac797874a1262f54e0f1cafcf28768c405bb7f1c003b12b8f11894fff4f682e3d4d45ef76935a0a676c660b58537c357df0cfda8748895" },
                { "lv", "6d7a1c06594bd2827a878efd3261669565958e87dbf78568ec72638e231ec995b031a2f663cfc093329b64ff60987e52cb6bf57f3c6b91d0f08cea46b1c2fc73" },
                { "mk", "9960673b68f632fdabe3aea78c8fe7c2a586e714eb799e1d26106b99a1d8b967ca45419a74023ec0221549e15acd24f468171ac61c9d85ab1c6b6038cff698b2" },
                { "mr", "01aebd48a51efe0231d921a18120ea903a42176f2c80e583f1fdf1ccff268bfc47ebfa63a8fcddd02b3577ad63259d80105409754bc0dc4c1e555270a63c4311" },
                { "ms", "c3c2048526f4d2c014555b199e8f8e8f192a8d79228dfa6751ff2b89672f92eb6329faa155c6217a6e11bd59a3156d3dfd8985aab2f184149fca5455490c79b0" },
                { "my", "04ba93a46f672c8e5b7fddb5c8932274cc14ecee84eaf28a68dd18a0750daf76092ec9be656979fe4aabf72915d84b9ad73d962d6a465aa8b714e4c659fa116f" },
                { "nb-NO", "a813344d888e7a8655454a9ec0db2d284a86bc62da7d2f8d8d5f2a039a0cb22b329251a67698c2be4a6cb6df1ddbce1220e37ccaa295eebd77e54e3f0064e3f9" },
                { "ne-NP", "b189cb6f5bebab9b9839e353f7141ac661baf97067a84906a9640a6080a628f16cc8e27fd871d667dcc14596bd0204ddfef7fd84603b6e99ba1cde9f028d877e" },
                { "nl", "9d2480ea5d6d409a25bfb81a045b489f54e42605bdd8d5c653043cfad8de92b40ba5d1c174eec70bd17a1eac49e6673b63e2256c8e7eebe8acd78d35ab313474" },
                { "nn-NO", "ac86ea79beb9c2c7ca58dcc0685c8ddbcfcd940356b09fda61b361abf6afd86059e8ca52ab9b647f7e0671856f759ef9637a54ff82d92f3644c8bfd6644d5ea0" },
                { "oc", "ee6cb40ed2646a867478549c6b7cd8c6d2fd6d90bd12b44e2ea1be9cc9f252eb265defd876b6d0efcb3c99862c93af70e37708801b5640ae083b3b6e210fac61" },
                { "pa-IN", "cdb4a65cdab810cdcbf05b6016c20eca2c6e96b2dfa6dfd4126da47f36465913841d64ffc8359e5431067c8871e74e98600b3f076741c6c9e6631388688d9ad6" },
                { "pl", "c84770e1736ba817a7286f62dce21072e47c1a9bb744fb59266dc7a123409b253d38518f7c55b6220d353751e1f7052440cee2632641c4b7dde5341b1acb06c4" },
                { "pt-BR", "b2b023f4558c87d1aa9326ddd6c60c98fcbe479866b26ed38e37f01055d3ec0ee7134fa3a87f74be4746ebd990f84b64162ad648fac2513d1ced2eeb9b53de94" },
                { "pt-PT", "67ed76552efbde8842af936dba0ce90fc36d8a31852039fa0ba39c79dd685133263252cae6206d8d8adfde81d9196b66ebcbe5b7fb70828689ee24a31b65c89c" },
                { "rm", "3efd15aa7733d9d9b8f44c7a95be03a06a711dd7d90a57131ac86dd6939e429231a513d98542c460a38fb14d60fabe19e7604e8c364607669bd7d4437f8e9297" },
                { "ro", "7981621d3d479123f5c28748e2bc621dad6b1e28633e3071c28a7cd44bc1e6a03c27391c57c6077d748d824193c1b6d737131a1037032884ef00ae1da25aa4ea" },
                { "ru", "c1ec91606214ba5a0fdb3bad8063689f59d60f5092443de4fd9d7f6446393662748d929fde4358a6cd84d98eaac97fd5f176e4898a57a62f0b05306906de885d" },
                { "sco", "97f1e46cec6e87afa8d1cebc74cd95bf625cf8a30ebb8a2430b0d65cdd98c4a91e949818a7c3e4dd2013ed0721873b40cab982f042187de7ce2c0f65c5eeaf32" },
                { "si", "ba7e508771ca0d20203b7abd09527b3f867788150b9f7523a18169175424716a535d1681a0d565fbd9f2a946ec32593835584197530c2db1ad000b893ee82e48" },
                { "sk", "271576126e9a7e63f3fc7f167e4e0de9be4649e9c6fb0499fb08a74bea529cb5cd699c48de0f34fcb386e8d3d6c412612f3ec806be415c6f7dd124c98454f8ed" },
                { "sl", "47fa100cead308c42dab3a7a5f18d3d8e8a1800163d195c1496aab9c188a059e78aa50dc9b97ca82c6e315471edabee0dc2b56c28f4f369307439c06ad5e50d3" },
                { "son", "b2e7bce01919e6da11a42d1b8b8cdc08e2d39e87e94d5fa89b76d99055f8838631dd09ec1033564c3462a0102c268146a91a4a126046d50e0970193d3eced2a3" },
                { "sq", "d70741ebf171dc0b1498650b18c10744d06a422042309d7744384d711c441e74992389724e316cf4fcb75dca63e266697ee634d32ef85f65275ac55609ffc2d7" },
                { "sr", "63226fac13e93de6e1edcdb01c08523fd914ed705ac29b725f847852afb48347f93c4b0572893111bc6939ddcd6a2d99ab6b5891abdf75a7cc8961320f9a8347" },
                { "sv-SE", "850022d9d76406e01c7ca8544180837f950a38267ab27cc08c91d71f77e03e3e716d5cc0c1abeae96c5023915c92fff872f16719b35dfdb6c3cdd5a89faf72ab" },
                { "szl", "ee26e40451695dc5d9a497c19cebb7206ec17a506abff8fc0790d22de9733f4ce00261cc2ac9a5ea01dac8c8e4eb82ae80afae65955be8d192eb112be078983d" },
                { "ta", "bf5376610ea4b6395240f710a9b392a38ff68bd3e41e14dc7443d7ada6bd4b000622ed39e6f81b1053bad4562dbbd3319f090d47f625674a5c03cf61dfe09b82" },
                { "te", "0ce54fccac39b1bb0f9757e9890b81139ae8dd3d43884de8cca021fcb62cc23b5d205ec9014b41fcbee62fac8511990abeac6bf354f60dbd4a4ff24f17aaf248" },
                { "th", "943cb419287b46da8366eb005ed220549ce9a22f0430c6bb7917038e6d20c21a37221a4cbd4c2e347dc5304f4fb4e2fd6ae1df674adccc2555676551557bfd36" },
                { "tl", "d054be6ee768f326ee7c026a4f2d15d58c6bd5f1579e4d61e21049c398f2f7f3f9a13189e36741ffc8a9798c69fc54edf9e003b6fb1b7fbe27f447d75c8016fd" },
                { "tr", "7d7cce56fe42b1ca9071b3a5a7ab695f7945c0aa55c7334cf3108f36764f3d61536ca1106c7de27b5eb6fec91a23004619bb8ec9712de97d65a302d4cdd5348e" },
                { "trs", "862a0a92e3455d3b8365ddd673b2738b1e69c9ca90476393919942871001f505f30f3058ef01d8463267b2392fc39935f165636f68989d5a79914bc72a588c8f" },
                { "uk", "0b6ff353cbec628fc7fb1004c920c7ee48750d3342091d4f0e2f91c0369d6a0c3b073d12350492e45043988b4524825e72ee14be88032446ee515a5c75de627f" },
                { "ur", "df65abc5d4034b9d14158e3fab07402fb113688d5aff9a6367cc44e47084c39e69fb3685813c61b3da9d85ec83af5628b979e7830d3fcfbcb8bf397bdc450f06" },
                { "uz", "84def8e2b388043779812d133559fb935451bcd59a6fec3fb197a3e5295d5878bbe6f0bc3b20ce07bd0e7c072a5b5858a2aebc37124b093ace070ca6fddb6a78" },
                { "vi", "6aaddc69f9b6c716f3943d7cb99557bfc53c5e952d09f90e6721b52760f2ec782be97d100112e0ac4bab576fc470518b971fcad1b87bfc63e2c1e4f60ea6d8b3" },
                { "xh", "92f3492dc107da41bbabe177c06a287fe1df194d690e8f488a766bb5d3f53ccfb29959774ecacbee298899374d526079100c3fca6ec3d2f6949163f8f069a2c0" },
                { "zh-CN", "4c11ba27906e357144693b8e261088ba72f2bbb02ab97c6477c7a465964268f59099529863d93d1367e96506ca155791ef944d7093ceed05424c2a67d45bbe40" },
                { "zh-TW", "ee3617006e960c81ade207f15077c27c3dc3c0c4fa6e5e4c141b1e760784b89dc6d3c911a1af3272bc4ef143c8de8c2a77304aef5196ed91e942efb518b8f90c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/106.0b8/SHA512SUMS
            return new Dictionary<string, string>(97)
            {
                { "ach", "5fdb6aff3e2b86f9e932dd20102b61cce412323026e8282dc41e4935cd76946a9943cefe536cb5e20633e61537b4849da633f502c088e2c97c74e187c39d95af" },
                { "af", "9dd463d1dce636285900f3ccdc93679dcad8b569b3d6d8e8422e3373b3ce34ed5ebe6989a922764480355791e0bb9871a940433e8d4970a471a801daa6342ade" },
                { "an", "ead59d797e74eaa128c8e0503c296ac066fb3368f0b8d82a638d42defc08aa96e01262d59aff0603157524df6bf991ef2c073fb41238e7a673d93128aecb5e40" },
                { "ar", "37fc5af81e7eded6c060d906a866de2bd0b5188e4cabaa5332c835784b3bb2367253cf71f26786100be52d55df9f0c8a76c798bef7adda04948f5d692161179f" },
                { "ast", "acff885eb5b634cba21669f5fd65521d41c199e8173a94c92c534062a5a96ccfccc5f32d5e6383083fc8bdfe20e91bfbe84baccec9fe200c68526356bb86ee23" },
                { "az", "c31f33e0a6593589265b5ffc6973bbf78a33d428089c5eed0b4327f2b7d966b3edaba18ca97a3064e5092542f62ea9ace4dc4a4228c34c42b4b7c61993561399" },
                { "be", "d2290ae17d691a73bce3b87a8cd6759b36df9afa5b391344ca9c5a9d5383e8dba611e91c0112fe0cd7ab9ce0ecee481b51dbd1bb2528f09a508731aaeee1bafd" },
                { "bg", "66879fb9363c2016dd97cdc49b43f143e342c58e0d1885c910a312181393d1fd1b02ddb244fab839f9d701ac4c4be8f368365c8366f74ef8484fc2931de0ca92" },
                { "bn", "f452f8c81c2f0b6bd3caa52a9cba99093b586daf226adfd5852db1f6572bc0b0127e0a4bbb7c0f57a0240dd77f513ef4279cdbd35e3a9b0e0cf386ce86d93907" },
                { "br", "d3647c7e9c87fc531691fd81bbe6e2a7e6482c0f67001545e8fbcbb610b6f2a8a3d73773b7a69c55acb18f802b93f30fa05f885e7fb713109aadee7db024d90e" },
                { "bs", "c924b0686ec9bc4ca1f9d416d26aed04e9979e4d04887d8980faabf4f7d2e3318829586f3228299e540346784c2d8ef688cbb29426e942b141e0f397fdeeb811" },
                { "ca", "4f18c923103095f27c46376f4b2d75c47cbca86a4eee9e890ff821562aa32e7186bb9b4ed2895379d6f61d400c0071607c8c5a702365ad2d82cd1d3aefe8c10a" },
                { "cak", "c909c60507a800202d6314751298fb9c753243c0b31d20949588fde86b74a2171e6473f7d844b241ec93d2fddc49f4e69b432f2610f3bb26f211d7b3a6c9d90d" },
                { "cs", "38d14b8929bffbe211dcf83e4f8b82a071e59a5feb85579286bd11b0fcc83bf308322371f0518a59a6193397529d77c68a91c3abefc59aad92ad866eb83580d3" },
                { "cy", "f241f07862d64c89a5dba73f7144c317e2e38ab9fa829c9afac8c30663bb8188ba068902f245be7874b30adb4e52e7704405e3f770dd936dd918dd4948b23e91" },
                { "da", "a5c7ac6615054aba637c94e2669341a5ec85aad0938c9ef58a308e5d655854c1b8f057390a28b96e1d1d05121f57fa4f8ce26cbb57ea0901c69456139b91fc8f" },
                { "de", "edc2571af8f50d8387ac30c90c812bb9dfe42a1b8fb709f833ff5ee53ddbceb83e36949fce8705fb4b29fee0d839f4a8bfca49183bbda9208a40505f4cb9679c" },
                { "dsb", "ffcde5e4cd0333ca404939f69959b175a3306cd75de070ab95d3aaaa04eae6271bf55ad0fc068872b6dc80a5b6be8e7e49d605ee5c74bf19996d8604be444f8b" },
                { "el", "a1a05ca56ef4f0d8d110f6e3dea98807cd22657c96f5cf50860aaf47463f17c207a06b4f4c2f38ad1fe3f089125e068917644fa44c2db7e5d2a8e375494b1630" },
                { "en-CA", "ae9a357378141569d2958d210672fd4f58b2c330a3c5ee379b28b3664e93c31bbd07dcfe6542b4b1deb25d659db7465793f07d7dc61effdf532449dbfbea2a08" },
                { "en-GB", "c680098d3c3ba1e91ee29caeeb1693fbb450ef399efbad2ad3ffd4eceb9850a57ced2ac244c18707e298c10958160272806936df98e59761ebc776037f4b5990" },
                { "en-US", "639997cfb3ae751008bf2e6e197421e0d5af9fb944d466100eb561909e145a9f3beb3ff53323f51c88dae1909273099f0a31c8cf643d9c70d76abb89143bdece" },
                { "eo", "c1080aa7b0fecb9d4adc069906c0e039ed94c8f4c3ba133e123efa9c8a787964fa0cc2b71745d5c1efe2c920580d916ed95c8d02202956c778ecf9be681edb15" },
                { "es-AR", "d722d05a8aac563f69224e13f76edbfb491202427af1910817fc4fb99199eb36d7b8c93a4958b83cf214b5320dd62d31607ce6e3d1869485dc3439148f7262db" },
                { "es-CL", "7c93126304e379c31f23a6a2353c220b3824db77efa1eb4c582f3f086078c1cc399a95040add8ef50477883d575b83aca8dc465a517e224ae2d540d92b4bdb06" },
                { "es-ES", "f93a92d155da7662f368538e81bffe5132fce5427804bc44c37852e83b59d357f9b5d5922e48b31ab07ee621f1243cce0061502a25a9d5c8abe18565b48ed789" },
                { "es-MX", "d8841c3abdcf8aef5be696b684363053d94653f824cf82b14e46972a0c065389e939b84252ac9640d0a5c98d2562e000154db75d45fb1e5994b7d76ee24e03c7" },
                { "et", "82eb7e8d1b07caee2576bc2cb8568ab89f011ab8d74f9a683662f452cc1b3b5d5e08d6db0ee7f718353972033ef13ecfa271fc7491de061ea05b489814ca5788" },
                { "eu", "0c76abecd076d40f5ac7e8afdd22394fbd6085f2c686e630c2514594496585d430e4b5f99de75f6946d8be107308e6f45e1a0a3a75918d86a2ee058ee0d27eb2" },
                { "fa", "586b5cf49122e4c40ca819933e36d0b899a1b78fc66508993b53e6ef5d5eee22e1fcb76d606789f9939c92cc5c3466d9ef7dd108bc25108ca94753c39c35e608" },
                { "ff", "7365821f4ea9e37bb1a9eb847fc2621ca7818921160418254cb3d592544a84151d6064766293cc6d8cf8f1ee1b4d6558e45d9690a4f57a17c39ecd57758cf9ce" },
                { "fi", "d83f9b8004e99b0f5af4ae59bd19df5894f31a94478d260f0836df8c441b128f39abc0426f20b1feac483f4f7b7e906b1f6bbf680f8c6e7fdb09552dc5d96ee7" },
                { "fr", "e00726d99326ae250fc2003477967c4aa15a22e2b7b47ca41f7a62a378f6b27216cb0d99575398c8c086c1345df59c411682e497be74cffdc4169a5eb96a8aae" },
                { "fy-NL", "a59bf3d07726ff34000723a1916eb49960506a6a59f776fbed24383d48ac72daa30a93c4861cdb2f8954fcf605cbd595c8f5a90db9c11d2103f6b9355c1f929e" },
                { "ga-IE", "776151293cd42db6a860d63bc8e973512e62357c57359b6941f60090543e10dbcbc8f9e7ec3e17512d62733858bdcedd3bcc0074c14c7ce3dc754b8f9f5886e1" },
                { "gd", "06e16fe8b1e3d62d6c3a8a402900b475e540d5cacd554a81232cb47ff45d33f092acf93e820af68daa8921960c4537dd07c5873b41f1f2b37598f1983ce2785e" },
                { "gl", "afd83af410fc6f4c3d57fd76bc282b6eed65dab36a609eb5956bbd5ba148bf99c652f25b3d541632e836f946a53c74bb166b66d0b76275042c0677b10732f42e" },
                { "gn", "454785996428ea8a1febb4d322d3c589d8e6d873a6ba0a63cfec043d10cfd693a67b3d7b2b0f1d1e21fedcacdf1e86d08006287582f68fb6a60bfad8090ec375" },
                { "gu-IN", "49949e7b83e3f17b80164325d5ecd84237c89dc5e7f831bedba549a5aa0154cd7787bb379ba91fc818ded6abe864432d3a7a8d3cca3ab699aa54c55a7574ab57" },
                { "he", "26d105a8dbff29b2dc4a52c254b109752a283c42ab9653683cc233cc10962872365309d321d53c974eb264795b4509db8cf2caf88c4015fce81405d3c7414a93" },
                { "hi-IN", "c81d8007fcf8a7cd0daaba9d3951b574a133bbe386f029ae8dbc00798adfc273aa9505945b12faa74c61ea64d514ba6f46a81c90a74219685256d49397847ee0" },
                { "hr", "b57ee5e587cb1d26e091d54b1138f4abf83c0a1f02dd2320b4acd1a3dd7d0e421edea3a3c48a4a34421749dcf762afb3c9762171a165f7a9928b392bd47b21b2" },
                { "hsb", "ba769511cdb6dadc7378edb31685cde8b9e29013c21f4fd2039f28a8618b25d6db4fcc4420839ea70a3dd18ce2bb7028c9a88a882ee543145776806b67bdf35a" },
                { "hu", "05a56dfe7c6b00a09d53d2916c03fdaebdeb2a414028bcb6dd1a5c594edb4722a5775d3e12e0f9387f3b0769fcfb13d01b00d6fa591f0c37a08579c6d3a36644" },
                { "hy-AM", "b69bbf01ecf31887011b23eafe94d12f9d76283c0d6cf4e01d040d9dbb94e57c91f102846b48e45333595d3d7e057f350c72d21400ee6e288d282f8986889086" },
                { "ia", "821123a0fa4c68c1138aba8f2554d6b791ab5b88f0027bac518b86984eb7938f35e09c02dde6a4048f01d434524362dd4b4553800d44e640b205a0e715bcbced" },
                { "id", "192936db9e79aa72ddd960b0dc69f999ba1e4b6565a69fc061a17e2f8a8f02cfd6b8f9b4c0af98ee63958dd958324572381787aebc728db905c0e175958d74db" },
                { "is", "bca95ca735d0e63a9c94c767825c2a14cfb9d131a327d9bec95abe40117f9f60a1c2e54914c00090691e822130197e8dc35676e6908061acd67e0cbcb0c33c9e" },
                { "it", "d0404616baaed4027d2ad8ab5fe7e6e8234dcbcf81c5ae5339e684170351afe16013ecfeb1e51d3727173fafa7d77c2eceb92a57e466109f5b1e660ddfcabb2e" },
                { "ja", "4111e2a1f6beec4f1ddca23f249b3e25ebf66cca97ec4391487636e1554682dfe82e65f55d1df56213549a027295330c676911dd395d998dce8b829e9071afb3" },
                { "ka", "91f4801b5a12c5ce454085acebc43f7ab1bc3d16b00cf517abdda6099268a8f99a54318602b8fac43e2dd4e81efa10f131dbca24912e9e2b563b769e1b1e6049" },
                { "kab", "4c41ef7a35ba7684c194d263ff08d725c5db29dadb97991e6622616ee49ecbc08521fc487aa75aed4a17f7555aecc58b3b6b5128210745f75554d5720003ba2b" },
                { "kk", "f0eb6b4c259fe72fb72d541201ce04724a3c348d522c914dca694cc96a9dba45dfb26e16eaff830062ee0a481a72795488b2caf82df6e575eb981d8cb2df2005" },
                { "km", "39ea33cec7d64f8d830f2e6c434b101ff0bc03141f0fd46085d17b3ddab8b9a34c9c4c3c5edc8187d19db753f9207260a0aadcd67769ecdfab6773ad33d540d4" },
                { "kn", "29d531d1bffe49bc8970b63b43a019a72662de169f045b575283dfe1721efbb880de42d98097e573875145a8fcd3de658d406126f6cd893bc029905f1e08918f" },
                { "ko", "4847f228cf9d9b777e5bf56049340af9f145d29e8658b053d86d4641fbb49905d44e3826614f91b5dbfde6133641f06d5495543d65c7bdbb3f4991ef4e405eeb" },
                { "lij", "18cb37e731e319f70122f41b88e19dfd3902d216af4988f5778205b22b7cb3af063b465c7c82d218b3366f666f9fefc19ed62776ed80218668d73bcb78d88846" },
                { "lt", "c49603e877560b7d4d930226c01c92f19ef69dcde5ee5689383156525ac1b8d9ea0d8f85bf69fbcc838af08cbbff6eff79194eacc84738e319e7ec117e077102" },
                { "lv", "cf4145441c00b128d35ad5587246f7edb0104fa9c1581c1f2d71b11f3f16783381c4d0d17b8434cd5b4afb5d9413075c2ba6391b4dc7460af3c195ba370b6ac0" },
                { "mk", "4a5734e75bfa3b72da82ec3c8c06fa4b8f6e9bf99ade96b41c40f5c2a74e4e9b3d22c8945af99db892436e89302a9c8c5e22328f8606065526ddd6391f173ddd" },
                { "mr", "996febecebfaa294aab196e8920bb0cd43f611e54ecaddd585310ea8cc23490c8c8d890814083ba9eb49c6c8892ae15cbf07aba0bf3e0deb7517b33ca7489c5a" },
                { "ms", "ad7dbd56b42c700370629d8537f4556f11786d516bf697143cff6c66f80469ef6e901f5d879bb51db663c8dd2546d674df50b9020c80a50ea3fd7086aa1a9eb7" },
                { "my", "a52357f79b407eaba44749460f66d65ed54f7cfe6bb93d0dca489721a7d3491d108a4549ad20774845de160c8a328b59fe65b7777ba07f1622c2fa0b80b772a6" },
                { "nb-NO", "ebd0ddb71c2be863d7f9c4b7c64205b195d42bd760d4234bab410f92a8178caf23838aeb38f983fa97cd7d34756a198ca65b0da12cb604ce3054330e7e7746be" },
                { "ne-NP", "03d61e81a72a36bf430838da79eb2f77d840d7c24700b9a772cd41a38b319ea7c83673d4e71e19cc22251059d7fe4017987b0b21ae1e2ee75c2d86b65e0d7ab6" },
                { "nl", "5ba9b15f9dfb7fdec6f4f567fea0009608bf14b9225da369ee3b4bf546445b23bf222118255f323f7cdcbfa8d033e615b222faff37c8435dd34158c06c8a85a3" },
                { "nn-NO", "316809a362b4065b68345f329822030a934cd23ca705718a0f136530079fb89fdd3525a206320042c1b1108474b81cfd229b992e5d220fe6fc42bd19e44eb0e2" },
                { "oc", "23ac47a94f8c2659fba3b54b0225c7bbb40abbd61f10b8ad879e87e2b61b2feb2bdb1a80bd0f149ff32c5b2298754976a19e919bb8cfa42fb0ecf53816cec17d" },
                { "pa-IN", "015b24c844fc5a9ef20fea48b1059cd111e9a9cc67e43fce1dfdc44d16f7f69c95407c00c79f70bcbe546a433d92b78961c2b597119eac48c891b22be55da93d" },
                { "pl", "fedd76f873aa2bab0b8f2b49bd9a395b314b6f97b917494b6049a64cfa04c45662b9b1370e4abbf407a859b2a7baa2a242932338bb9cd85d8ce6fda60df83abd" },
                { "pt-BR", "9ffb6c8a56975cbcd85f273b472e59aa8b770d1a094119460dafecc92b3c293041c99e5d66ff05d2810b1a4e4d0e47daf41c88a0875570316a92df2e1221fd00" },
                { "pt-PT", "6b8b5e1ccb85fd21c1b6db99479616fe6be0a3b30c90ebc9a761c5cba4fee543e575f4223a09a79831a16629fd3995aebbca9cbee0addcda4ac78cf36d56a3c2" },
                { "rm", "1206b1d4fb56eb91a934aeb36500aa9bc022d2fc434ce72a42143e675dbdee9dbd72da08af9c224e817c9616baf8a7edb419f0d51faac38be2174f520ffc0247" },
                { "ro", "e15283ec87e85948ca420bc1f0a25cbc2c4fee73244ea89bffbb3f75c37a6e574a460d4f0f4ca6c9cc760209522c44e07688f1ba04034caed2afcbe7b0a08046" },
                { "ru", "db2f7442fff993997faf4920111abfc41e87762434dec7857a111cae4805b11239d128105f4f0636e898857799032566bd38eeeabde3d859d94e4836d1f88b67" },
                { "sco", "3f5faf1c68c819397d22d192d7aac43162b7533fa0f6adcbc5e6c6d9d090629ccc48205fe393b8ac9790d6eff2004811e7e9ad94792372ab8cf41dcf1c45e8eb" },
                { "si", "2b148cdfb631e426fc981ff6e391cdc3001b6257fa81d1e6a0f6c5cd9cee287e9695ff7b7ee8f8dedb0b194f2691ce1a65aac383b1f6e6c140d961c6666f1294" },
                { "sk", "a5fe8510d416f23b2c024895ab49136b97cb6dbcf1268cf98e8c2cfeca5bc3eb27417851de1037fbb0c0c33856f2abf6d309a4887ce0703d639a1c64e96d883c" },
                { "sl", "ccb87b76f04f5612cb85e9cdcf2abb8f0cbe8b0dd9e255a39fb2190530c7f5822eb307f2a67426f189f82f213b36b6fed503e2341d975a1836778a2f363eff03" },
                { "son", "42bb7e6690217007415fac577f2d5c91c77345a8777811a61b711cb20e36b0f1b4e2d2ff2289cce3cc4bf2a1ea33282f1a195042be1168e3d0deb6dddc01f364" },
                { "sq", "e33aed8fb317f7b9f6cd0230d5989f605c51456bff90e8f7d9f71633ee2a919dc3d36c60962253a92ad0eacb09690dd08b8d7b3e6cf1d51e31ca0ab465ad4c77" },
                { "sr", "6369ff5ead099235a11dfb4d8cf724fcd0b8626e0d1a27dd913f48364f8b40fd6ff065975cd7d61856bf2513fa5c957a34dc72556d101078fb2e368a1fd0ec3e" },
                { "sv-SE", "be1c013c2535283789268de40608ffb01e6ac2f8313a9c0dec97d55a7ae1a84d804330255c3bc41c93d7e4b122ab9097f786d20ca44721a3b03b61b2d69d9601" },
                { "szl", "687610897d83b0126534a56eb469b90eaf25fe5239d14db1ba7147c869bf6de2f0e1f7affdd9c1efab0affb2b7965f50f547920472ffd8f12408a663fd7aca04" },
                { "ta", "ce90b10608702a9279ff8bfa18815b97ee7034a52f3f8909520341cc87a2202a9b5255f293478db725bd06d86a562d1d7ec061e3f93146b1d59fd8a6b9842c9f" },
                { "te", "0015efceba7a41254057612f3450d1e22a6ef1c88d405e9375d4ad2951c2ec48480d7bc98a778890f30f124033f4557c99f7408e78270709ca4599e3995d1633" },
                { "th", "f35a9c5088924bb839c69f6a73818eafdeeb3027dda7d1d9327cf824ebef8f9bece3d10484a0706a662d45f9e5d860317e4ba63b5c15adbc8f8ef8025084dfd8" },
                { "tl", "678e34326d301b3f6e3eca4aeb2a253af800e3eb4b495285218e920664f11a1035b727993e1a3d6fce7993b51ece55ae2dbdb224edc144c6ab00f282fbd3ba3a" },
                { "tr", "8294e7d4566ab5d4f5fd9b3f57b17347787248329e4ce24302adf5ded19696e240efb092f4b7040fa0304a757dc4f41cfab3d58109e1f74fd033d775c015f451" },
                { "trs", "4048d89325e6e6f52b61c9cf21d8c77d00edf00f2ef80339c67fe6d08f5a043b4186a09fba091c813c7c2b7d4b1d35c77ca57ef4bc9beede93d9bec906029067" },
                { "uk", "891d2ae8898218b05e49f238eb8fe61ff3cc3273bd5ad4901724344cb2da7547a63ff5bb89cbb26fd2f473ef6f42a9e0b1b97201053827df4e53048deeb24cdb" },
                { "ur", "848e2a657b14a88bda4cff4699b4fbc397b36367e93dd1f29e3b6f9f1afdc1822f452d191f5e36213fbcf2214ddb9e89874aead9e492fbfb9e1b23db95662d3e" },
                { "uz", "914bc7bd6d4fc6c450e94ce798e665503443acae308777f919967eefe0369867196ab08d8a24487ee556e5841ff7661598c167568978a702ee765932756654e7" },
                { "vi", "680bf64a8d28456206ffe97f81f6586ca41442c5565832022dce318ad2fcbd071ca6450ca94d6dc3a278657ba89410e0d5a39191cf14c882e61ddc28e439dc3f" },
                { "xh", "7609e9f2366b3ab14add647cd59347f5a29488fd07cdffdd7a0e38a4ae0225d1b74f1632ab51b6656ec661095dfa664998261b22e1ad0d758226ca0de91affe0" },
                { "zh-CN", "b60a4e575f0e75bad2a767059239ee47674947d38dd942522752b0798bba695576a4761d9223cf364c86bf6c4a5583459f419dbbc94c24e409d58721908affad" },
                { "zh-TW", "a7c44f7de217ae9abdb312b1d53a210afe342688c4ad6bb7aa65ae65f7dbc42d30f3920a853e224200a78b7acbd1b5873d5abd01ccda7f4713965ae3c7ab4e67" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
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
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
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
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
                    if (newerVersion == currentVersion)
                    {
                        checksumsText = sha512SumsContent;
                    }
                }
                catch (Exception ex)
                {
                    logger.Warn("Exception occurred while checking for newer"
                        + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                    return null;
                }
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
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
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
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


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
